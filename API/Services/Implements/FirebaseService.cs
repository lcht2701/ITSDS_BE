﻿using API.Services.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Firebase.Storage;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class FirebaseService : IFirebaseService
{
    private readonly IRepositoryBase<User> _userRepository;
    private static string Apikey = "AIzaSyDSp2BGBcsS282cPTJSxUzFoW2PKWzAZ0A";
    private static string Bucket = "itsds-v1.appspot.com";
    private static string AuthEmail = "admin@gmail.com";
    private static string AuthPassword = "admin123";

    public FirebaseService(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> UploadFirebaseAsync(MemoryStream stream, string fileName)
    {
        string link = "";
        var auth = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(Apikey));
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

    public async Task<string> CreateFirebaseUser(string email, string password)
    {
        UserRecordArgs args = new UserRecordArgs()
        {
            Email = email,
            EmailVerified = true,
            Password = password,
            Disabled = false,
        };

        UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        return userRecord.Uid;
    }

    public async Task<bool> UpdateFirebaseUser(string oldMail, string newMail, string? newPassword)
    {
        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(oldMail);
        UserRecordArgs args = new UserRecordArgs()
        {
            Uid = userRecord.Uid,
            Email = newMail,
            EmailVerified = true,
            Disabled = false,
        };

        if (newPassword != null)
        {
            args.Password = newPassword;
        }

        UserRecord userUpdated = await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
        return userUpdated is not null;
    }

    public async Task<bool> RemoveFirebaseAccount(int userId)
    {
        bool check = false;
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));

        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
        #region Remove User Document
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(userRecord.Uid);
        await docRef.DeleteAsync(); 
        #endregion
        #region Remove Firebase Auth Account
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(userRecord.Uid);
        #endregion
        return check;
    }

    public async Task CreateUserDocument(User user, string uid)
    {
        string createdAtTime = new DateTimeOffset((DateTime)user.CreatedAt!).ToUnixTimeMilliseconds().ToString();
        string lastActiveTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        string about = $"I am {DataResponse.GetEnumDescription(user.Role)}";
        string fullname = $"{user.FirstName} {user.LastName}";
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(uid);

        // Create a data object for the document
        Dictionary<string, object> data = new()
        {
            { "id", uid },
            { "name", fullname ?? "" },
            { "email", user.Email! ?? "" },
            { "image", user.AvatarUrl! ?? "" },
            { "created_at", createdAtTime ?? "" },
            { "last_active", lastActiveTime ?? "" },
            { "about", about ?? "" },
            { "is_active", true },
            { "push_token", "" },
        };
        await docRef.SetAsync(data);
    }

    public async Task UpdateUserDocument(User user)
    {
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(user.Id.ToString());
        string newFullname = $"{user.FirstName} {user.LastName}";
        string newAbout = $"I am {DataResponse.GetEnumDescription(user.Role)}";

        // Get the existing user document data
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            // Extract existing data
            Dictionary<string, object> existingData = snapshot.ToDictionary();

            // Update only the fields that need to be changed
            existingData["name"] = newFullname ?? existingData["name"];
            existingData["email"] = user.Email ?? existingData["email"];
            existingData["image"] = user.AvatarUrl ?? existingData["image"];
            existingData["about"] = newAbout ?? existingData["about"];

            // Update the Firestore document with the modified data
            await docRef.UpdateAsync(existingData);
        }
        else
        {
            // Handle the case where the user document doesn't exist
            // You can choose to create a new document or handle the error as needed.
        }
    }
}
