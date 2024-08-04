using DeleteInactiveMembers.Tables;
using SqlSugar;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DeleteInactiveMembers.Modules
{
    public class Listener
    {
        public TelegramBotClient BotClient { get; set; }
        private Repo.Member MemberDB { get; set; }
        public List<long> Admins { get; set; }
        public User Me { get; set; }
        private Dictionary<string, Func<Message, string, string[], Task>> commandsFunction = new();

        public Listener(Repo.Member memberDb)
        {
            MemberDB = memberDb;
            HttpClient httpClient;
            if (Env.USE_PROXY)
            {
                Log.Information("使用 {proxy} 代理服务器登录Telegram...", Env.PROXY);
                WebProxy webProxy = new WebProxy(Env.PROXY, true);
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                httpClientHandler.Proxy = webProxy;
                httpClientHandler.UseProxy = true;
                httpClient = new HttpClient(httpClientHandler);
            }
            else
            {
                Log.Information("登录Telegram...");
                httpClient = new();
            }
            BotClient = new TelegramBotClient(Env.TG_TOKEN, httpClient);
            Me = BotClient.GetMeAsync().Result;
            Admins = BotClient.GetChatAdministratorsAsync(Env.WORK_GROUP).Result.Select(x => x.User.Id).ToList();
            BotClient.StartReceiving(async (c, u, t) =>
            {
                using var __defer = new Defer(() => { });
                await OnUpdate(c, u, t);
            }, OnError);
            Log.Information("成功作为 {fullname}(@{username}) 登录", Me.GetFullName(), Me.Username);
            
        }

        private Task OnError(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Log.Error(exception, "与 Telegram 服务器通信时发生错误");
            return Task.CompletedTask;
        }

        private async Task OnUpdate(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is not { } message) return;
            if (message.From == null) return;
            long chatID = message.Chat.Id;
            User user = message.From;
            var member = new Member(user.Id, user.GetFullName(), message.MessageId);
            using var __存储非管理员的成员活跃数据 = new Defer(() =>
            {
                if (user.IsBot) return;
                if (Admins.Contains(user.Id)) return;
                MemberDB.Storageable(member).ExecuteCommand();
                Log.Debug("更新 Member {@member}", member);
            });
            try
            {
                //处理纯文字消息
                if (message.Text != null && message.Text.Length > 0)
                {
                    if (message.Text.StartsWith('/'))
                    {
                        string[] parts = message.Text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        string command = parts[0].Split('@')[0]; // 获取命令，去除@botname部分
                        string[] args = parts.Length > 1 ? parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
                        if (Env.WORK_GROUP != chatID && !allowedPrivateCommand.ContainsKey(command)) throw new NotSupportedException("当前仅在指定群组工作喵");
                        await OnCommand(message, command, args);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "{name}({id}): {message}",user.GetFullName(),user.Id,message.Text);
                var tx = await BotClient.SendTextMessageAsync(chatID, $"被玩坏了 {ex.GetType().Name}: {ex.Message}");
                tx.DeleteLater();
            }
        }

        private async Task OnCommand(Message message, string command, string[] args)
        {
            if (message.Chat.Type != ChatType.Private && !message.Text!.Contains(Me.Username!)) return;
            User user = message.From!;
            long chatID = message.Chat.Id;
            //忽略机器人的消息
            if (user.IsBot) throw new NotSupportedException("不能使用频道马甲喵");

            if (!commandsFunction.ContainsKey(command)) return;
            await commandsFunction[command].Invoke(message, command, args);
        }

        private List<BotCommand> botCommands = new();
        Dictionary<string, bool> allowedPrivateCommand = new();

        public void RegisterCommand(string command, Func<Message, string, string[], Task> func, string? desc = null,bool allowPrivateChat = true)
        {
            Log.Information("正在注册指令 {c} - {desc}", command, desc ?? "No Description");
            if (allowPrivateChat) allowedPrivateCommand[command] = true;
            commandsFunction.Add(command, func);
            if (desc != null)
                botCommands.Add(new()
                {
                    Command = command,
                    Description = desc,
                });
        }

        public async Task SetMyCommandsAsync()
        {
            await BotClient.SetMyCommandsAsync(botCommands, new BotCommandScopeChat() { ChatId = Env.WORK_GROUP });
            await BotClient.SetMyCommandsAsync(botCommands, new BotCommandScopeAllPrivateChats());
        }
    }
}