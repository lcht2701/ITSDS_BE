using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Persistence.Services.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageFirebaseAsync(MemoryStream stream, string fileName);
    }
}
