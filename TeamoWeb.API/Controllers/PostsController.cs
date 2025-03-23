using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;
using Teamo.Core.Specifications.Posts;
using Teamo.Infrastructure.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;
using static System.Net.Mime.MediaTypeNames;

namespace TeamoWeb.API.Controllers
{
    [Route("api/groups/{groupId}/posts")]
    public class PostsController : BaseApiController
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly INotificationService _notiService;
        private readonly IDeviceService _deviceService;
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;


        public PostsController
        (
            IPostService postService, 
            IUserService userService,
            IGroupService groupService,
            INotificationService notiService,
            IDeviceService deviceService,
            IUploadService uploadService,
            IConfiguration config
        )
        {
            _postService = postService;
            _userService = userService;
            _groupService = groupService;
            _notiService = notiService;
            _deviceService = deviceService;
            _uploadService = uploadService;
            _config = config;
        }

        [Cache(1000)]
        [HttpGet]
        public async Task<ActionResult<Pagination<PostDto>>> GetPostsAsync([FromRoute]int groupId, [FromQuery]PostParams postParams)
        {
            postParams.GroupId = groupId;
            var spec = new PostSpecification(postParams);
            var posts = await _postService.GetPostsAsync(spec);
            var countSpec = new PostSpecification(postParams, false);
            var totalPosts = (await _postService.GetPostsAsync(countSpec)).Count();
            var postDtos = posts.Any() ? posts.Select(p => p.ToDto()).ToList() : new List<PostDto?>();
            var pagination = new Pagination<PostDto>(postParams.PageIndex, postParams.PageSize, totalPosts, postDtos);
            return Ok(pagination);
        }

        [Cache(1000)]
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostByIdAsync(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound(new ApiErrorResponse(404, "Post not found."));    
            return Ok(post.ToDto());
        }

        [InvalidateCache("/posts")]
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<PostDto>> CreatePostAsync([FromRoute]int GroupId,[FromForm] PostToUpsertDto postDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized();

            var post = postDto.ToEntity();
            if(postDto.Document != null)
            {
                var document = postDto.Document;
                // Upload and get download Url
                var documentUrl = await _uploadService.UploadFileAsync(
                    document.OpenReadStream(),
                    document.FileName,
                    document.ContentType,
                    _config["Firebase:PostDocumentsUrl"]);

                // Update document url
                post.DocumentUrl = documentUrl;
            }

            post = await _postService.CreatePost(post, user.Id, GroupId);

            var groupMembers = await _groupService.GetAllGroupMembersAsync(GroupId);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = post.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateNewPostMessage(deviceTokens, post.Group, post.Id, user, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Post created successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(post.ToDto());    
        }

        [InvalidateCache("/posts")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<PostDto>> UpdatePostAsync(int id,[FromForm] PostToUpsertDto postDto)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if(post == null) return NotFound(new ApiErrorResponse(404, "Post not found.")); 
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized();

            post = postDto.ToEntity(post);
            if (postDto.Document != null)
            {
                var document = postDto.Document;
                // Upload and get download Url
                var documentUrl = await _uploadService.UploadFileAsync(
                    document.OpenReadStream(),
                    document.FileName,
                    document.ContentType,
                    _config["Firebase:PostDocumentsUrl"]);

                // Update document url
                post.DocumentUrl = documentUrl;
            }
            post = await _postService.UpdatePost(post, user.Id);

            var groupMembers = await _groupService.GetAllGroupMembersAsync(post.GroupId);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = post.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateUpdatedPostMessage(deviceTokens, post.Group, post.Id, user, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Post updated successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(post.ToDto());
        }

        [InvalidateCache("/posts")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> DeletePostAsync(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound(new ApiErrorResponse(404, "Post not found."));
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized();

            await _postService.DeletePost(post, user.Id);
            return Ok("Successfully delete this post");
        }
        [Cache(1000)]
        [HttpGet("/api/posts")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<Pagination<PostDto>>> GetTopPosts([FromQuery] PagingParams pagingParams)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized();

            var groupSpec = new GroupSpecification(new GroupParams { StudentId = user.Id });
            var groupIds = (await _groupService.GetGroupsAsync(groupSpec)).Select(g => g.Id);
            
            var (posts, totalPosts) = await _postService.GetUserPosts(groupIds, pagingParams);
            
            var postDtos = posts.Any() ? posts.Select(p => p.ToDto()).ToList() : new List<PostDto?>();
            var pagination = new Pagination<PostDto>(pagingParams.PageIndex, pagingParams.PageSize, totalPosts, postDtos);
            return Ok(pagination);

        }

        private static FCMessage CreateNewPostMessage(List<string> tokens, 
            Group group, int postId, User user, string status)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "New Post",
                body = $"{user.FirstName + " " + user.LastName} added a new post in {group.Name}.",
                data = new Dictionary<string, string>
                {
                    { "type", "new_post" },
                    { "groupId", group.Id.ToString() },
                    { "postId", postId.ToString() },
                    { "status", status}
                }
            };
        }

        private static FCMessage CreateUpdatedPostMessage(List<string> tokens, 
            Group group, int postId, User user, string status)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Updated Post",
                body = $"{user.FirstName + " " + user.LastName} updated a post in {group.Name}.",
                data = new Dictionary<string, string>
                {
                    { "type", "updated_post" },
                    { "groupId", group.Id.ToString() },
                    { "postId", postId.ToString() },
                    { "status", status}
                }
            };
        }
    }
}
