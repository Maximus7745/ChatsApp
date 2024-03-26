using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Java.Lang;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace ChatsApp
{
    public class JsonHelper
    {
        const string fileName = "chats.json";
        static string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        static string filePath = Path.Combine(documentsPath, fileName);
        public static async Task<List<MessageJava>> ReadJSON()
        {
            List<MessageJava> messagesJava = new List<MessageJava>();
            List<Message> messages = new List<Message>();
            if (File.Exists(filePath))
            {
                List<Message> allMessages = new List<Message>();
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    allMessages = await JsonSerializer.DeserializeAsync<List<Message>>(fs);
                }
                allMessages = allMessages.OrderByDescending(m =>
                {
                    DateTime dateTime;
                    if (DateTime.TryParseExact(m.Date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        return dateTime;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }).ThenByDescending(m =>
                {
                    TimeSpan time;
                    if (TimeSpan.TryParse(m.Time, out time))
                    {
                        return time;
                    }
                    else
                    {
                        return TimeSpan.MinValue;
                    }
                }).ToList();
                foreach (var m in allMessages.GroupBy(m => m.ChatID))
                {
                    
                     messages.Add(m.First());
                    
                }
            }
            else
            {
                messages = new List<Message>()
                {
                    new Message("chat1", "User", "Hi", "3/26/2019 12:00:00 AM", "02:17:46.0389600"),
                    new Message("chat2", "User", "Hi", "3/26/2019 12:00:00 AM", "02:17:46.0389600")
                };
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync<List<Message>>(fs, messages);
                }
            }
            foreach(var message in messages)
            {
                messagesJava.Add(new MessageJava(message));
            }
            return messagesJava;
        }

        public static async Task<List<MessageJava>> ReadMessagesById(Guid id)
        {
            List<MessageJava> messagesJava = new List<MessageJava>();
            List<Message> messages = new List<Message>();
            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    messages = await JsonSerializer.DeserializeAsync<List<Message>>(fs);
                }
            }
            messages = messages.Where(m => m.ChatID == id).ToList();
            messages.OrderBy(m =>
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(m.Date, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }).ThenBy(m =>
            {
                TimeSpan time;
                if (TimeSpan.TryParse(m.Time, out time))
                {
                    return time;
                }
                else
                {
                    return TimeSpan.MinValue;
                }
            }).ToList();
            foreach (var message in messages)
            {
                messagesJava.Add(new MessageJava(message));
            }
            return messagesJava;
        }
        public static async Task<bool> SaveMessages(Message message, Message messageResponse)
        {
            List<Message> messages = new List<Message>();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    messages = await JsonSerializer.DeserializeAsync<List<Message>>(fs);
                }
                messages.Add(message);
                messages.Add(messageResponse);
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync<List<Message>>(fs, messages);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
            return true;
        }

        public static async Task<bool> SaveMessage(Message message)
        {
            List<Message> messages = new List<Message>();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    messages = await JsonSerializer.DeserializeAsync<List<Message>>(fs);
                }
                messages.Add(message);
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync<List<Message>>(fs, messages);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
            return true;
        }


        public class Message
        {
            public Guid MessageID { get; set; }
            public Guid ChatID { get; set; }
            public string ChatName { get; set; }
            public string SenderName { get; set; }
            public string Text { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public Message(MessageJava message)
            {
                MessageID = message.MessageID;
                ChatID = message.ChatID;
                ChatName = message.ChatName;
                SenderName = message.SenderName;
                Text = message.Text;
                Date = message.Date;
                Time = message.Time;
            }
            [JsonConstructor]
            public Message(Guid messageID, Guid chatID, string chatName, string senderName, string text, string date, string time)
            {
                MessageID = messageID;
                ChatID = chatID;
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
            public Message(string chatName, string senderName, string text, string date, string time)
            {
                MessageID = Guid.NewGuid();
                ChatID = Guid.NewGuid();
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
            public Message(Guid chatID, string chatName, string senderName, string text, string date, string time)
            {
                MessageID = Guid.NewGuid();
                ChatID = chatID;
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
        }
        public class MessageJava : Java.Lang.Object
        {
            public Guid MessageID { get; set; }
            public Guid ChatID { get; set; }
            public string ChatName { get; set; }
            public string SenderName { get; set; }
            public string Text { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public MessageJava(Message message)
            {
                MessageID = message.MessageID;
                ChatID = message.ChatID;
                ChatName = message.ChatName;
                SenderName = message.SenderName;
                Text = message.Text;
                Date = message.Date;
                Time = message.Time;
            }
            public MessageJava(Guid messageID, Guid chatID, string chatName, string senderName, string text, string date, string time)
            {
                MessageID = messageID;
                ChatID = chatID;
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
            public MessageJava(string chatName, string senderName, string text, string date, string time)
            {
                MessageID = Guid.NewGuid();
                ChatID = Guid.NewGuid();
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
            public MessageJava(Guid chatID, string chatName, string senderName, string text, string date, string time)
            {
                MessageID = Guid.NewGuid();
                ChatID = chatID;
                ChatName = chatName;
                SenderName = senderName;
                Text = text;
                Date = date;
                Time = time;
            }
        }

    }
}