using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Services.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName);
    }
}
