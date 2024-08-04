using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

namespace DeleteInactiveMembers
{
    public static class Helper
    {
        public static string GetFullName(string? firstname, string? lastname, string fallback = "Blank")
        {
            if (string.IsNullOrWhiteSpace(firstname) && string.IsNullOrWhiteSpace(lastname))
            {
                return fallback;
            }
            firstname = firstname ?? "";
            lastname = lastname ?? "";
            return (firstname + " " + lastname).Trim();
        }
        public static string GetFullName(this Telegram.Bot.Types.User user)
        {
            return GetFullName(user.FirstName,user.LastName);
        }
        public static string HtmlEscape(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            char[] chars = text.ToCharArray();
            int len = text.Length;
            StringBuilder sb = new StringBuilder(len + (len / 10));

            for (int i = 0; i < len; i++)
            {
                switch (chars[i])
                {
                    case '<': sb.Append("&lt;"); break;
                    case '>': sb.Append("&gt;"); break;
                    case '&': sb.Append("&amp;"); break;
                    default: sb.Append(chars[i]); break;
                }
            }

            return sb.ToString();
        }
        public async static Task<Message> FastReply(this TelegramBotClient b,Message message,string text)
        {
            try
            {
                return await b.SendTextMessageAsync(message.Chat.Id, text, replyToMessageId: message.MessageId,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            catch (Exception)
            {
                return await b.SendTextMessageAsync(message.Chat.Id, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        public async static Task<Message> FastReply(this Message message, string text)
        {
            var b = Hosting.GetRequiredService<Listener>().BotClient;
            return await FastReply(b, message, text);
        }
        public static void DeleteLater(this Message message, int second = 15)
        {
            var b = Hosting.GetRequiredService<Listener>().BotClient;
            DeleteLater(b,message, second);
        }
        public static void DeleteLater(this TelegramBotClient b, Message message, int second = 15)
        {
            _ = Task.Run( async()=>
            {
                Thread.Sleep(TimeSpan.FromSeconds(second));
                await b.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            });
            
        }
    }
}
