using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace aTTagirl
{
    class Handler
    {
        public static bool excuse, swicher;
        public static string excusestring;
        public static int scoretype;
        public static CallbackQuery callbackQueryMain;
        public static int callbackQueryMessageToDelete;
        public static List<int> messageIDstring = new List<int>();

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!, excuse: excuse),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)

            };
            try
            {
                await handler;
            }
             catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, bool excuse = false)
        {

            Console.WriteLine($"Receive message type: {message.Type}");
            //Console.WriteLine($"The message was sent with id: {message.Sticker.FileId}");
            if (message.Type == MessageType.Sticker)
            {
                Console.WriteLine($"The message was sent with id: {message.Sticker}");
                return;
            }
            else if (message.Type != MessageType.Text)
                return;
            else if (excuse)
            {
               await GameWithExcuse(botClient, message);
                return;
            }



            var action = message.Text!.Split(' ')[0] switch
            {
                "/start" => MainMenu(botClient, message),
                "/info" => Usage(botClient, message),
                //"/remove" => RemoveKeyboard(botClient, message), 
                //"/photo" => SendFile(botClient, message),
                //"/request" => RequestContactAndLocation(botClient, message),
                _ => Usage(botClient, message)
            };
            Message sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            static async Task<Message> GameWithExcuse(ITelegramBotClient botClient, Message message)
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, callbackQueryMessageToDelete);
                excusestring = message.Text;
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                return await BotOnRewardReceived(botClient, callbackQueryMain, scoretype);
            }

            static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                const string usage = "Usage:\n" +
                                     "/start   - запустить бота\n" +
                                     "Набери любое сообщение с описанием того что ты сегодня сделала чтобы получить награду";
                //"/remove   - remove custom keyboard\n" +
                //"/photo    - send a photo\n" +
                //"/request  - request location or contact";
                Message messageone = await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage,
                                                            replyMarkup: new ReplyKeyboardRemove());
                return messageone;
            }
        }
        //static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
        //{
        //    ReplyKeyboardMarkup replyKeyboardMarkup = new(
        //        new[]
        //        {
        //            new KeyboardButton[] { "1.1", "1.2" },
        //            new KeyboardButton[] { "2.1", "2.2" },
        //        })
        //    {
        //        ResizeKeyboard = true
        //    };

        //    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                                text: "Choose",
        //                                                replyMarkup: replyKeyboardMarkup);
        //}

        //static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message)
        //{
        //    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                                text: "Removing keyboard",
        //                                                replyMarkup: new ReplyKeyboardRemove());
        //}

        //static async Task<Message> SendFile(ITelegramBotClient botClient, Message message)
        //{
        //    await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

        //    const string filePath = @"Files/tux.png";
        //    using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

        //    return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
        //                                          photo: new InputOnlineFile(fileStream, fileName),
        //                                          caption: "Nice Picture");
        //}

        //static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message)
        //{
        //    ReplyKeyboardMarkup RequestReplyKeyboard = new(
        //        new[]
        //        {
        //        KeyboardButton.WithRequestLocation("Location"),
        //        KeyboardButton.WithRequestContact("Contact"),
        //        });

        //    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                                text: "Who or Where are you?",
        //                                                replyMarkup: RequestReplyKeyboard);
        //}



        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            long userID = callbackQuery.From.Id;
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");
            var handler = callbackQuery.Data switch
            {
                "Roulet" => BotOnRouletteReceived(botClient, callbackQuery),
                "Atta" => AttaMenu(botClient, callbackQuery.Message),
                "Medal" => MedalMenu(botClient, callbackQuery.Message),
                "Wisdom" => BotOnWisdomReceived(botClient, callbackQuery),
                "Statistics" => BotOnStatReceived(botClient, callbackQuery),
                "Info" => BotOnInfoReceived(botClient, callbackQuery),
                "Setup" => BotOnSetupReceived(botClient, callbackQuery),
                "Atta1" => BotAskForExcuse(botClient, callbackQuery, 1),
                "Atta2" => BotAskForExcuse(botClient, callbackQuery, 2),
                "Atta3" => BotAskForExcuse(botClient, callbackQuery, 3),
                "Medal1" => BotAskForExcuse(botClient, callbackQuery, 4),
                "Medal2" => BotAskForExcuse(botClient, callbackQuery, 5),
                "Medal3" => BotAskForExcuse(botClient, callbackQuery, 6),
                "ExcuseYes" => BotWaitForexcuse(botClient, callbackQuery, scoretype),
                "ExcuseNo" => BotOnRewardReceived(botClient,callbackQuery, scoretype),
                "AskClear" => BotOnDeleteRecieved(botClient, callbackQuery),
                "ClearPlayer" => BotOnDeletePlayer(botClient, callbackQuery),
                "StatAll" => BotOverallStat(botClient, callbackQuery),
                "StatExcuse" => BotStatWithExcuses(botClient, callbackQuery),
                "BackWithDelete" => BotDeleteStatRow(botClient, callbackQuery),
                _ => MainMenu(botClient, callbackQuery.Message),

                //_ => await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Received {callbackQuery.Data}")
            };

            //await botClient.SendTextMessageAsync(
            //    chatId: callbackQuery.Message.Chat.Id,
            //    text: $"Received {callbackQuery.Data}");
        }

        private static async Task<Message> BotOnDeletePlayer(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            List<Score> scores = await Program.ScoreStat.GetScoresList();
            foreach (Score score in scores)
            {
                if (score.PlayerID == callbackQuery.From.Id)
                { 
                    await Program.ScoreStat.DeleteScoreAsync(score);
                }
            }
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Я замел все следы🥷🏿",
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotOnDeleteRecieved(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: "ClearPlayer"),
                        InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: "Setup"),
                    },                    
                });
            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Точно удаляем",
                                                        replyMarkup: inlineKeyboard);           
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
              
        static async Task<Message> MainMenu(ITelegramBotClient botClient, Message message)
        {
            if (message.MessageId == callbackQueryMessageToDelete)
            { 
                callbackQueryMessageToDelete = 0;
            }
            else
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);                
            }

            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Крути рулетку\n🎰", callbackData: "Roulet"),
                    },
                    // second row                    
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Похвали меня\n ☺️", callbackData: "Atta"),                        
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Дай медальку\n 🏅", callbackData: "Medal"),
                    },
                    // third row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Cовет\n🔮", callbackData: "Wisdom"),
                        InlineKeyboardButton.WithCallbackData(text: "Статистика\n📊", callbackData: "Statistics"),
                    },
                    // fourth row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Инфо\n💡", callbackData: "Info"),
                        InlineKeyboardButton.WithCallbackData(text: "Настройки\n⚙️", callbackData: "Setup"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "<ins>---------</ins><i>Главное меню</i><u>-------</u> ", parseMode : ParseMode.Html,
                                                        replyMarkup: inlineKeyboard);

        }

        private static async Task<Message> BotOnRouletteReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {

            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            //await Task.Delay(500);
            //Message MessageOne = await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "3", parseMode: ParseMode.MarkdownV2);
            //await Task.Delay(700);
            //await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, MessageOne.MessageId);
            //Message MessageTwo = await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "2", parseMode: ParseMode.MarkdownV2);
            //await Task.Delay(700);
            //await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, MessageTwo.MessageId);
            //Message MessageThree = await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "1", parseMode: ParseMode.MarkdownV2);
            //await Task.Delay(700);
            //await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, MessageThree.MessageId);
            Random random = new Random();
            int attNumber = random.Next(1, 4);
            Games.GameNewRound(playerID: callbackQuery.From.Id, attNumber: attNumber, $"Награда от бота {DateTime.Now}", false) ;
            string attName = Games.AttaSwitch(attNumber);
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });            
            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: $"||Сегодня ты \n {attName}||", parseMode: ParseMode.MarkdownV2,
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotOnInfoReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "<b>bold <i>italic bold <s>italic bold strikethrough</s> <u>underline italic bold</u></i> bold</b>", parseMode: ParseMode.Html,
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotOnStatReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            // Simulate longer running task
            await Task.Delay(500);           

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Общую статистику", callbackData: "StatAll"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Что я там понаписала", callbackData: "StatExcuse"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Чего изволите",
                                                        replyMarkup: inlineKeyboard);
        }
        private static async Task<Message> BotStatWithExcuses(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            // Simulate longer running task
            await Task.Delay(500);            
            List<Score> scores = await Program.ScoreStat.GetScoresList();
            string tempString;
            Message MessageOne;
                foreach (Score score in scores)
                {
                if (score.ScoreReasonManual && score.PlayerID == callbackQuery.From.Id)
                {
                    tempString = score.DateRecieved.ToString() + " ты " + score.ScoreName.ToString() + "Ты написала\n" + score.ScoreReason.ToString();
                    MessageOne = await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: tempString);
                    messageIDstring.Add(MessageOne.MessageId);
                }
                }
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "BackWithDelete"),
                    },
                });
            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Возвращаемся?", replyMarkup: inlineKeyboard);
        }
        private static async Task<Message> BotDeleteStatRow(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            foreach (int msgid in messageIDstring)
            {
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, msgid);
            }
            messageIDstring.Clear();
            callbackQueryMessageToDelete = callbackQuery.Message.MessageId;
            swicher = true;
            return await MainMenu(botClient, callbackQuery.Message);
        }
        private static async Task<Message> BotOverallStat(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            // Simulate longer running task
            await Task.Delay(500);
            string statistics = "Все что нажито непосильным трудом:";
            int count;
            string scoreName = "";
            Console.WriteLine("/nDone");
            List<Score> scores = await Program.ScoreStat.GetScoresList();
            for (int i = 1; i < 6; i++)
            {
                count = 0;
                foreach (Score score in scores)
                {
                    if (score.ScoreType == i && score.PlayerID == callbackQuery.From.Id)
                    {
                        scoreName = score.ScoreName;
                        count++;
                    }
                }
                if (count == 0)
                { }
                else
                {
                    statistics = statistics + $"\n{scoreName} - {count}";
                }
            }

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: statistics,
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotAskForExcuse(ITelegramBotClient botClient, CallbackQuery callbackQuery, int Attatype)
        {            
            scoretype = Attatype;
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                { 
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Просто так", callbackData: "ExcuseNo"),
                        InlineKeyboardButton.WithCallbackData(text: "Я расскажу", callbackData: "ExcuseYes"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });
            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Расскажешь за что хвалить?",
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotWaitForexcuse(ITelegramBotClient botClient, CallbackQuery callbackQuery, int Attatype)
        {
            excuse = true;
            callbackQueryMain = callbackQuery;
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });
           Message message = await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Ну рассказывай",
                                                        replyMarkup: inlineKeyboard);
            callbackQueryMessageToDelete = message.MessageId;
            Message message1 = null;            
            return message1;
        }

        private static async Task<Message> BotOnRewardReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, int Attatype)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            if (excuse != true)
            {
                await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            }
            else
            {
                
            }
            
            // Simulate longer running task
            await Task.Delay(500);
            if (excuse)
            {
                Games.GameNewRound(playerID: callbackQuery.From.Id, attNumber: Attatype, excusestring, excuse);
                excuse = false;
            }
            else
            {
                Games.GameNewRound(playerID: callbackQuery.From.Id, attNumber: Attatype, $"Награда от бота {DateTime.Now}", excuse);                               
            }

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });
            string finalstring;
            if (Attatype < 4)
            {
                finalstring = "Ты сегодня";
            }
            else
            {
                finalstring = "Вот твоя медалька";
            }

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: $"||{finalstring} \n {Games.AttaSwitch(Attatype)}||", parseMode: ParseMode.MarkdownV2,
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotOnWisdomReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {                    
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Непередаваемый\nполезный\nи мудрый\nохуительный совет",
                                                        replyMarkup: inlineKeyboard);
        }

        private static async Task<Message> BotOnSetupReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Очистить статистику", callbackData: "AskClear")
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                        text: "Настройки",
                                                        replyMarkup: inlineKeyboard);
        }


        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler
        static async Task<Message> AttaMenu(ITelegramBotClient botClient, Message message)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Молодец☺️", callbackData: "Atta1")                      
                    },
                    // second row                    
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Умничка😊", callbackData: "Atta2")
                    },
                    // third row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Солнышко☀️", callbackData: "Atta3"),                        
                    },
                    // forth row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });
           
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Кем хочешь быть",
                                                        replyMarkup: inlineKeyboard);
            
        }
        static async Task<Message> MedalMenu(ITelegramBotClient botClient, Message message)
        {
            //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🥇", callbackData: "Medal1")
                    },
                    // second row                    
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🏅", callbackData: "Medal2")
                    },
                    // third row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🎖", callbackData: "Medal3"),
                    },
                    // forth row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "Back"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Какую медаль ты хочешь",
                                                        replyMarkup: inlineKeyboard);
        }
    }
}
