using Teamo.Core.Entities;

namespace Teamo.Core.Interfaces.Services
{
    public interface IUploadService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, 
            string contentType, string directory);
    }
}