using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;

namespace aTTagirl
{
    //struct BotUpdate
    //{
    //    public string text;
    //    public long id;
    //    public string? username;
    //}
    class Program
    {
        static DataBase scoreStat;
        public static DataBase ScoreStat
        { 
            get 
            {
                if (scoreStat == null)
                {
                    scoreStat = new DataBase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PlayersDatabase.db3"));
                }
                return scoreStat;
            }
        }        
    private static string token  = "Token";
        private static TelegramBotClient Bot;

        public static async Task Main()
        {
            Bot = new TelegramBotClient(token);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                    UpdateType.Unknown,
                    UpdateType.ChosenInlineResult,
                    UpdateType.InlineQuery,
                    UpdateType.CallbackQuery,
                }
            };
            Bot.StartReceiving(
                Handler.HandleUpdateAsync,
                Handler.HandleErrorAsync,
                receiverOptions,
                cancellationToken
                );

            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            }
    }
}

