using System;
using System.IO;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchIntegration
{
    public static class TwitchManager
    {
        public static string UserName { get; set; }
        public static string UserSecret { get; set; }

        public static string ConfigFilePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "TwitchIntegration.txt");

        public static void Initialize()
        {
            if (!File.Exists(ConfigFilePath))
            {
                Save();
            }

            var lines = File.ReadAllLines(ConfigFilePath);
            foreach (var line in lines)
            {
                var lineSplitted = line.Split('=');
                var name = lineSplitted.ElementAtOrDefault(0);
                var value = lineSplitted.ElementAtOrDefault(1);

                if (name == nameof(UserName))
                {
                    UserName = value;
                }
                else if (name == nameof(UserSecret))
                {
                    UserSecret = value;
                }
            }
        }

        public static void Configure()
        {
            Initialize();

            ConfigureUserName((userNameChanged) =>
                ConfigureUserSecret((userSecretChanged) =>
                {
                    if (!userNameChanged && !userSecretChanged) return;

                    Test(Save);
                })
            );
        }

        private static void ConfigureUserName(Action<bool> doneAction)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    titleText: "Twitch integration",
                    text: $"Enter your Twitch username (current is '{UserName}'):",
                    isAffirmativeOptionShown: true,
                    isNegativeOptionShown: true,
                    affirmativeText: "Save",
                    negativeText: "Cancel",
                    affirmativeAction: userName =>
                    {
                        if (!string.IsNullOrEmpty(userName))
                        {
                            UserName = userName;
                        }

                        doneAction?.Invoke(true);
                    },
                    negativeAction: null
                )
            );
        }

        private static void ConfigureUserSecret(Action<bool> doneAction)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    titleText: "Twitch integration",
                    text: $"Enter your Twitch secret (current is '{UserSecret}'):",
                    isAffirmativeOptionShown: true,
                    isNegativeOptionShown: true,
                    affirmativeText: "Save",
                    negativeText: "Cancel",
                    affirmativeAction: userSecret =>
                    {
                        if (!string.IsNullOrEmpty(userSecret))
                        {
                            UserSecret = userSecret;
                        }

                        doneAction?.Invoke(true);
                    },
                    negativeAction: null
                )
            );
        }

        public static void Save()
        {
            File.WriteAllLines(ConfigFilePath, new[]
            {
                $"{nameof(UserName)}={UserName}",
                $"{nameof(UserSecret)}={UserSecret}",
            });
        }

        public static void Test(Action successAction)
        {
            try
            {
                var credentials = new ConnectionCredentials(UserName, UserSecret);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
            
                var customClient = new WebSocketClient(clientOptions);
                var twitchClient = new TwitchClient(customClient);
                twitchClient.Initialize(credentials);
                twitchClient.Connect();

                InformationManager.ShowInquiry(
                    new InquiryData(
                        titleText: "Twitch integration",
                        text: "Your Twitch account is now linked!",
                        isAffirmativeOptionShown: true,
                        isNegativeOptionShown: false,
                        affirmativeText: "OK",
                        negativeText: null,
                        affirmativeAction: () => { },
                        negativeAction: () => { }
                    )
                );

                successAction?.Invoke();
            }
            catch
            {
                InformationManager.ShowInquiry(
                    new InquiryData(
                        titleText: "Twitch integration",
                        text: "Something went wrong with the connection. Double-check your credentials and your Internet connection.",
                        isAffirmativeOptionShown: true,
                        isNegativeOptionShown: false,
                        affirmativeText: "OK",
                        negativeText: null,
                        affirmativeAction: () => { },
                        negativeAction: () => { }
                    )
                );
            }
        }
    }
}
