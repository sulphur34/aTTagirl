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
        private static string token  = "5234460777:AAGa_a6V2k42DtybJPbmUR1rO3VXD1_oKJ4";
        private static TelegramBotClient Bot;
                      
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient(token);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
                );
            Bot.AnswerCallbackQueryAsync
            //var me = await Bot.GetMeAsync();
            //Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            Task HandleErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
            {
                throw new NotImplementedException();
            }
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken arg3)
            {
                var chatID = update.Message.Chat.Id;
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery:
                        //await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id,);
                        switch (update.CallbackQuery.Data)
                        {
                            case "Roulet":
                                { await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Data, "works", true); }
                                //{ await botClient.SendTextMessageAsync(chatId: chatID, text: "Roulette"); }
                                break;
                            case "Wisdom":
                                { await botClient.SendTextMessageAsync(chatId: chatID, text: update.CallbackQuery.Data); }
                                break;
                            case "Medal":
                                { await botClient.SendTextMessageAsync(chatId: chatID, text: update.CallbackQuery.Data); }
                                break;
                            case "Statistics":
                                { await botClient.SendTextMessageAsync(chatId: chatID, text: update.CallbackQuery.Data); }
                                break;
                            case "Info":
                                { await botClient.SendTextMessageAsync(chatId: chatID, text: update.CallbackQuery.Data); }
                                break;
                        }    
                        break;
                    case UpdateType.InlineQuery:

                        break;
                    case UpdateType.Message:
                        {
                            if (update.Message!.Type != MessageType.Sticker)
                                await botClient.SendTextMessageAsync(chatId: chatID, text: "Pick your Game", replyMarkup: FirstMenu());
                            else if (update.Message!.Type == MessageType.Sticker)
                            {
                                Message sendMessage = await botClient.SendTextMessageAsync(
                                chatId: chatID,
                                text: "StickerID: " + update.Message.Sticker.FileId);
                            }
                                
                        }
                        break;
                    case UpdateType.EditedMessage:
                        break;                    
                }

                
                
               
                
            }

            //Bot = new TelegramBotClient(token);
            //Bot.StartReceiving();
            //Bot.OnMessage += OnMessageHandler;

            //Console.ReadLine();
            //Bot.StopReceiving();
        }



        private static InlineKeyboardMarkup FirstMenu()
        {

            InlineKeyboardMarkup inlineKeyboard = new(new[]
     {
        // first row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Крутить рулетку🎰", callbackData: "Roulet"),
            InlineKeyboardButton.WithCallbackData(text: "Получить медаль🏅", callbackData: "Medal"),
            InlineKeyboardButton.WithCallbackData(text: "Мудрый совет🔮", callbackData: "Wisdom"),
        },
        // second row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Статистика📊", callbackData: "Statistics"),
            InlineKeyboardButton.WithCallbackData(text: "Инфо💡", callbackData: "Info"),
        },
    });


            //new List<List<InlineKeyboardButton>>
            //{

            //    //new List<InlineKeyboardButton>{new InlineKeyboardButton(text = "1",  ), new InlineKeyboardButton(text : "2" ), new InlineKeyboardButton(text : "3" ) },
            //    //new List<InlineKeyboardButton>{new InlineKeyboardButton(text: "4" ), new InlineKeyboardButton(text: "5" ), new InlineKeyboardButton(text : "6" ) }
            //}

            return inlineKeyboard;
        }

        //private static async void OnMessageHandler(object sender, MessageEventArgs e)
        //{
        //    var msg = e.Message;
        //    if (msg.Text != null)
        //    {
        //        Console.WriteLine($"New message: {msg.Text}");
        //        await Bot.SendTextMessageAsync(msg.Chat.Id, msg.Text, replyMarkup: GetButtons());
        //        //var stic = await Bot.SendStickerAsync(
        //        //    chatId: msg.Chat.Id,
        //        //    sticker: "https://tlgrm.ru/_/stickers/1d0/17d/1d017d53-40d1-4c83-bcc6-3ffa97df3a73/2.webp");
        //    }
        //}
    }
}
