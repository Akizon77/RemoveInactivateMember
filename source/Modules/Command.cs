using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            l.RegisterCommand("me", func: MeCommand, "获取我的活跃信息");
        }

        private async Task MeCommand(Message message, string command, string[] args)
        {
            message.DeleteLater(20);
            if (Admins.Contains(message.From!.Id))
            {
                (await message.FastReply("是狗管理！")).DeleteLater();
                return;
            }
            var id = message.From!.Id;
            try
            {
                var member = MemberDB.Queryable().Where(x => x.Id == id).First();
                string tx = $"<a href=\"tg://user?id={message.From.Id}\" >{message.From.GetFullName().HtmlEscape()}</a> 上次冒泡是在\n" +
                    $"<code>{member.LastActiveTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz")}</code>";
                (await message.FastReply(tx)).DeleteLater();
            }
            catch (Exception e)
            {
                Log.Warning(e, "新用户 {name}({id}) ",message.From.GetFullName(),message.From.Id);
                string tx = $"<a href=\"tg://user?id={message.From.Id}\" >{message.From.GetFullName().HtmlEscape()}</a> 现在冒了个泡！";
                (await message.FastReply(tx)).DeleteLater();
            }
            
        }
    }
}