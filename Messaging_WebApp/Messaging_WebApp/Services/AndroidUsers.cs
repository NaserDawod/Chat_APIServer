namespace Messaging_WebApp.Services;
using System.Collections.Generic;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
public class AndroidUsers
{
    public static Dictionary<string, string> map = new Dictionary<string, string>();
    public static bool c = false;
    
    public static void addAndroidUser(string username, string token)
    {
        if (map.ContainsKey(username)) {
            map[username] = token;
        }
        else {
            map.Add(username, token);
        }
    }

    public static void sendNotifcation(string from, string to, string content) {
        if (!c) {
            FirebaseApp.Create(new AppOptions() {
                Credential = GoogleCredential.FromFile(
                    "private_key.json")
            });
            c = true;
        }
        var registrationToken = map[to];
        var message = new Message() {
            Token = registrationToken,
            Notification = new Notification() {
                Title = from,
                Body = content
            }
        };
        string response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
        Console.WriteLine("Successfully sent message: " + response);
    }
}
