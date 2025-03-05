using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Teamo.Core.Interfaces.Services;

namespace Teamo.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        private readonly StorageClient _storageClient;
        private readonly IConfiguration _config;

        public UploadService(IConfiguration config, StorageClient storageClient)
        {
            _storageClient = storageClient;
            _config = config;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string directory)
        {
            var bucketName = _config["Firebase:StorageBucket"];

            // Create object name with directory path
            var objectName = string.IsNullOrEmpty(directory)
                ? $"{Guid.NewGuid()}_{fileName}"
                : $"{directory.TrimEnd('/')}/{Guid.NewGuid()}_{fileName}";

            var dataObject = await _storageClient.UploadObjectAsync(
                bucketName,
                objectName,
                contentType,
                fileStream);

            // Check if the upload was successful
            if (dataObject == null) 
            {
                return null;
            }

            // Return the Firebase Storage URL with the directory path
            return $"{_config["Firebase:FirebaseStorageUrl"]}{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }
    }
}