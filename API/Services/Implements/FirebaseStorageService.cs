using API.Services.Interfaces;
using Domain.Exceptions;
using Firebase.Auth;
using Firebase.Storage;

namespace API.Services.Implements;

public class FirebaseStorageService : IFirebaseStorageService
{
    private static string Apikey = "AIzaSyDSp2BGBcsS282cPTJSxUzFoW2PKWzAZ0A";
    private static string Bucket = "itsds-v1.appspot.com";
    private static string AuthEmail = "itsds@gmail.com";
    private static string AuthPassword = "itsds@123";

    public async Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName)
    {
        string link = "";
        var auth = new FirebaseAuthProvider(new FirebaseConfig(Apikey));
        var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

        var cancellation = new CancellationTokenSource();

        var task = new FirebaseStorage(Bucket, new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
            ThrowOnCancel = true,
        })
            .Child(fileName)
            .PutAsync(stream, cancellation.Token);

        try
        {
            link = await task;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }

        return link;
    }
}
