using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.Collections.ObjectModel;

namespace BKNova.Services;

public class FcmService
{
    private readonly FirebaseMessaging messaging;

    public FcmService(IConfiguration config)
    {
        // Initialize FirebaseApp once using service account file path from configuration
        try
        {
            var serviceAccountPath = config["Firebase:ServiceAccountPath"];
            if (!string.IsNullOrWhiteSpace(serviceAccountPath) && System.IO.File.Exists(serviceAccountPath))
            {
                // avoid creating multiple apps
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountPath)
                    });
                }
            }
            else
            {
                // If not configured with file, try default initialization (for environments where credentials are provided)
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("FCM init error: " + ex.Message);
        }

        messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task<string?> SendNotificationAsync(string token, string title, string body, IDictionary<string, string>? data = null)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var message = new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = data != null ? new ReadOnlyDictionary<string, string>(data) as IReadOnlyDictionary<string, string> : null
        };

        try
        {
            var result = await messaging.SendAsync(message);
            return result; // message id
        }
        catch (Exception e)
        {
            Console.WriteLine("FCM send error: " + e.Message);
            return null;
        }
    }
}
