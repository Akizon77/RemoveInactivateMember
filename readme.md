# 移除Telegram群组内的不活跃用户 Remove Inactive Users from Telegram Group

## 运行 Running

拉取仓库 Pull the repository:

```shell
git clone https://github.com/Akizon77/RemoveInactivateMember
cd RemoveInactivateMember
```

使用 docker-compose 运行 Run with docker-compose


```shell
# 在前台运行 Run in the foreground
docker-compose up
# 在后台运行 Run in the background
docker-compose up -d
```



## 配置

使用 `docker-compose.yml` 内的环境变量

Using environment variables in docker-compose.yml

环境变量优先级大于 `.env` 文件

Environment variables have a higher priority than the .env file.

```properties
# Enable debug mode.
# 使用调试模式输出更多信息
DEBUG = true

# Whether to use a proxy for network requests.
# 是否使用代理进行网络请求。
USE_PROXY = true

# Proxy address, currently only supports SOCKS5 protocol.
# 代理地址，目前仅支持 SOCKS5 协议。
# Example: socks5://127.0.0.1:1080
# 例如: socks5://127.0.0.1:1080
PROXY = socks5://127.0.0.1:12612

# Telegram bot token.
# Telegram 机器人令牌。
TG_TOKEN = 

# Which group to run the bot
# 机器人运行的群组
WORK_GROUP = 

# User inactivity timeout in days (d), hours (h), minutes (m), or seconds (s). 
# After a user has been inactive for a specified period, the bot will remove them from the group. 
# 用户不活跃超时时间，以天 (d)、小时 (h)、分钟 (m) 或秒 (s) 为单位。
# 当用户在指定时间段内不活跃时，机器人会将他们从群组中移除。
# Example: 10s, 5m, 1h, 1d
# 例如: 10s, 5m, 1h, 1d
TIMEOUT = 15d

# Interval for checking user inactivity, in days (d), hours (h), minutes (m), or seconds (s).
# 检查用户不活跃状态的时间间隔，以天 (d)、小时 (h)、分钟 (m) 或秒 (s) 为单位。
# Example: 10s, 5m, 1h, 1d
# 例如: 10s, 5m, 1h, 1d
INTERVAL = 5m

# Database type: sqlite or mysql
# 数据库类型: sqlite 或 mysql
DB_TYPE = sqlite

# If DB_TYPE is sqlite, the database file will be saved at ./dim.db. 
# This option has no effect if DB_TYPE is mysql.
# 如果 DB_TYPE 为 sqlite，数据库文件将保存在 ./dim.db。
# 如果 DB_TYPE 为 mysql，则此选项无效。
DB_FILE = ./members.db

# If DB_TYPE is mysql, this should be a connection string in the format:
# Server=1.1.1.1;Port=3306;Database=MyTable;Uid=admin;Pwd=123@@@;
# This option has no effect if DB_TYPE is sqlite.
# 如果 DB_TYPE 为 mysql，则应为以下格式的连接字符串:
# Server=1.1.1.1;Port=3306;Database=MyTable;Uid=admin;Pwd=123@@@;
# 如果 DB_TYPE 为 sqlite，则此选项无效。
DB_CONNECTION_STRING =
```
