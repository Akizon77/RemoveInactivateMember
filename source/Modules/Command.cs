using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeleteInactiveMembers.Modules
{
    public class Command
    {
        private Repo.Member MemberDB { get; set; }
        private TelegramBotClient BotClient { get; set; }
        private List<long> Admins { get; set; }

        public Command(Listener l, Repo.Member m)
        {
            MemberDB = m;
            BotClient = l.BotClient;
            Admins = l.Admins;
            l.RegisterCommand("me",  MeCommand, "上次冒泡是什么时候捏");
            l.RegisterCommand("about", AboutCommand, "关于");
        }

        private async Task AboutCommand(Message message, string arg2, string[] arg3)
        {
            message.DeleteLater(15);
            (await BotClient.SendTextMessageAsync(message.Chat.Id,
                "移除不活跃的群成员 By @AkizonChan",
                replyMarkup:new InlineKeyboardMarkup(new InlineKeyboardButton("Github") { Url = "https://github.com/Akizon77/RemoveInactivateMember" }) ,
                parseMode:ParseMode.Html,
                disableWebPagePreview:true))
                .DeleteLater(15);
        }

        private async Task MeCommand(Message message, string command, string[] args)
        {
            message.DeleteLater();
            if (Admins.Contains(message.From!.Id))
            {
                (await message.FastReply("是狗管理！惹不起！")).DeleteLater();
                return;
            }
            var id = message.From!.Id;
            try
            {
                var member = MemberDB.Queryable().Where(x => x.Id == id).First();
                string tx = $"<a href=\"tg://user?id={message.From.Id}\" >{message.From.GetFullName().HtmlEscape()}</a> 上次冒泡是在\n" +
                    $"<code>{member.LastActiveTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz")}</code>";
                (await message.FastReply(tx)).DeleteLater( );
            }
            catch (NullReferenceException e)
            {
                Log.Information("新用户 {name}({id}) ",message.From.GetFullName(),message.From.Id);
                string tx = $"<a href=\"tg://user?id={message.From.Id}\" >{message.From.GetFullName().HtmlEscape()}</a> 现在冒了个泡！";
                (await message.FastReply(tx)).DeleteLater();
            }
            catch(Exception e)
            {
                Log.Warning(e,"出乎意料的异常！");
                string tx = $"<a href=\"tg://user?id={message.From.Id}\" >{message.From.GetFullName().HtmlEscape()}</a> 现在冒了个泡！";
                (await message.FastReply(tx)).DeleteLater();
            }
            
            
        }
    }
}