﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Posts;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class PostsController : BaseApiController
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public PostsController(IPostService postService, IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Pagination<PostDto>>> GetPostsAsync(PostParams postParams)
        {
            var spec = new PostSpecification(postParams);
            var posts = await _postService.GetPostsAsync(spec);
            var countSpec = new PostSpecification(postParams, false);
            var totalPosts = (await _postService.GetPostsAsync(countSpec)).Count();
            var postDtos = posts.Any() ? posts.Select(p => p.ToDto()).ToList() : new List<PostDto>();
            var pagination = new Pagination<PostDto>(postParams.PageIndex, postParams.PageSize, totalPosts, postDtos);
            return Ok(pagination);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostByIdAsync(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();    
            return Ok(post.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePostAsync(PostToUpsertDto postDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorize"));

            var post = postDto.ToEntity();
            post.GroupMemberId = user.Id;   
            post = await _postService.CreatePost(post);   
            return Ok(post.ToDto());    
        }
    }
}
