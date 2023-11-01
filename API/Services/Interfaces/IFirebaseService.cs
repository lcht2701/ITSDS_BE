namespace API.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<bool> SignUp(string email, string password);
        Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName);
    }
}
