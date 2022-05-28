# FirstEntryAwardBot
Бот для выдачи вознаграждений за первое подключение к серверу

## Как установить бота:
Создайте в папке с ботом файл `appsettings.json` следующего содержания:
```
{
  "Token": "YOUR_TOKEN",
  "LogChannelId": "YOUR_LOG_CHANNEL_ID",
  "AccountConnected": "0",
  "ConnectionStrings": {
    "Default": "server=SERVER_IP;user=USERNAME;password=PASSWORD;database=DATABASE_NAME;port=PORT;Connect Timeout=5"
  }
}
```
Соответственно:
- `Token` - указываете токен своего бота
- `LogChannelId` - айди канала, куда будет писать бот при получение ключа (ему нужны будут права на просмотри написания сообщений в канал)
-  `AccountConnected`- кол-во дней, как долго аккаунту нужно быть на сервере, чтобы получить награду
- `ConnectionStrings: Default:` - данные о базе данных, где будет храниться информация.

Чтобы база данных работала, в нее нужно будет вставить SQL файл - [Вот этот](https://github.com/Paladic/FirstEntryAwardBot/blob/master/sqlForCreateBd.sql)

## Команды:
### /награда-получить
> Выдает награду (работает один раз)

### /оп-награда-поиск
> Ищет пользователя в БД, и проверяет есть ли у него награда

### /оп-награда-просмотр
> Показывает все награды и, если есть их получатели - то и их

### /оп-награда-добавить
> Добавляет введенные администратором награды