using DeleteInactiveMembers;
using DeleteInactiveMembers.Repo;
using Telegram.Bot;


using var __defer = new Defer(() => { Log.Information("Good bye~"); });
if (Env.DEBUG)
    Log.Logger = new LoggerConfiguration().WriteTo.Console().Enrich.FromLogContext()
        .MinimumLevel.Debug().CreateLogger();
else
    Log.Logger = new LoggerConfiguration().WriteTo.Console().Enrich.FromLogContext()
        .MinimumLevel.Information().CreateLogger();
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Log.Fatal((Exception)e.ExceptionObject, "Global Unhandled Exception occurred.");

Hosting.Register(s => s.AddSerilog());
Hosting.Register(new Database().DbClient);
Hosting.Register<Listener>(); 
Hosting.Register<Command>();
Hosting.Register<Member>();
Hosting.Register<Deleted>();
Hosting.Build();
var l = Hosting.GetRequiredService<Listener>();
Hosting.GetRequiredService<Command>();
_ = l.SetMyCommandsAsync();

var membersDb = Hosting.GetRequiredService<Member>();
var deletedMembersDb = Hosting.GetRequiredService<Deleted>();
membersDb.InitHeader();
deletedMembersDb.InitHeader();

//membersDb.Storageable(new DeleteInactiveMembers.Tables.Member() { Id = 6490522426 ,Name = "ＫＹＯＫＯ", LastActiveTime = DateTime.Parse("2024/07/04 15:27:16")}).ExecuteCommand();

await UnBan();
while (true)
{
    Thread.Sleep(Env.INTERVAL);
    l.Admins = l.BotClient.GetChatAdministratorsAsync(Env.WORK_GROUP).Result.Select(x => x.User.Id).ToList();
    Log.Debug("开始检查不活跃用户");
    var deadline = DateTime.UtcNow - Env.TIMEOUT;
    try
    {
        var inactiveMembers = membersDb.Queryable().Where(x => deadline > x.LastActiveTime).ToList();
        inactiveMembers.ForEach(async x =>
        {
            membersDb.Delete(x.Id);
            if (!l.Admins.Contains(x.Id))
                await Ban(x);
        });
        Log.Information("删除了 {0} 位不活跃用户", inactiveMembers.Count);
        deletedMembersDb.Storageable(inactiveMembers.Select(x => new DeleteInactiveMembers.Tables.Deleted() { Id = x.Id, Name = x.Name, LastActiveTime = x.LastActiveTime }).ToList()).ExecuteCommand();
    }
    catch (Exception e)
    {
        Log.Error(e, "删除用户时出错");
    }
}

async Task Ban(DeleteInactiveMembers.Tables.Member member)
{
    Log.Debug("封禁 Member {@member}", member);
    var m1 =await l!.BotClient.SendTextMessageAsync(Env.WORK_GROUP,
                $"<a href=\"tg://user?id={member.Id}\" >{member.Name.HtmlEscape()}</a> 因 {Env.Get("TIMEOUT")} 不活跃而被移出群组"
                , parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    try 
    {
        await l.BotClient.BanChatMemberAsync(Env.WORK_GROUP, member.Id, DateTime.UtcNow + TimeSpan.FromDays(3));
    }
    finally 
    {
        var aTimer = new System.Timers.Timer(TimeSpan.FromMinutes(1));
        aTimer.Elapsed += (_, _) =>
        {
            _ = UnBan();
        };
        aTimer.AutoReset = false;
        // 启动计时器
        aTimer.Enabled = true;
    }

    
}
async Task UnBan()
{
    var dm = deletedMembersDb.Queryable().ToList();
    foreach (DeleteInactiveMembers.Tables.Deleted x in dm)
    {
        try
        {
            Log.Debug("解除封禁 Member {@member}", x);
            deletedMembersDb.Delete(x.Id);
            await l.BotClient.UnbanChatMemberAsync(Env.WORK_GROUP, x.Id);
        }
        finally { }
    }

}