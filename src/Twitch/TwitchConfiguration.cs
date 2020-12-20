using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchIntegration.Twitch
{
    public class TwitchConfiguration
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user OAuth password.
        /// </summary>
        public string UserOAuthPassword { get; set; }

        /// <summary>
        /// Full path to config file.
        /// </summary>
        public string ConfigFilePath => System.IO.Path.Combine(Utilities.GetConfigsPath(), "TwitchIntegration.txt");

        /// <summary>
        /// Constructor.
        /// </summary>
        public TwitchConfiguration()
        {
            if (!File.Exists(ConfigFilePath))
            {
                Save();
            }

            Load();
        }

        /// <summary>
        /// Load Twitch credentials from configuration file.
        /// </summary>
        protected void Load()
        {
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
                else if (name == nameof(UserOAuthPassword))
                {
                    UserOAuthPassword = value;
                }
            }
        }

        /// <summary>
        /// Save Twitch credentials in configuration file.
        /// </summary>
        protected void Save()
        {
            File.WriteAllLines(ConfigFilePath, new[]
            {
                $"{nameof(UserName)}={UserName}",
                $"{nameof(UserOAuthPassword)}={UserOAuthPassword}",
            });
        }

        /// <summary>
        /// Ask the user to enter username and OAuth password.
        /// </summary>
        public void Configure()
        {
            ConfigureUserName((userNameChanged) =>
                ConfigureUserOAuthPassword((userOAuthPasswordChanged) =>
                {
                    if (!userNameChanged && !userOAuthPasswordChanged) return;

                    Test(Save);
                })
            );
        }

        /// <summary>
        /// Ask the user to enter the username.
        /// </summary>
        /// <param name="doneAction">Action to make when dialog is closed.</param>
        private void ConfigureUserName(Action<bool> doneAction)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    titleText: "Twitch integration",
                    text: $"Enter your Twitch bot username (current is '{UserName}'):",
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

        /// <summary>
        /// Ask the user to enter the OAuth password.
        /// </summary>
        /// <param name="doneAction">Action to make when dialog is closed.</param>
        private void ConfigureUserOAuthPassword(Action<bool> doneAction)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    titleText: "Twitch integration",
                    text: $"Enter your Twitch bot OAuth password (current is '{UserOAuthPassword}'):",
                    isAffirmativeOptionShown: true,
                    isNegativeOptionShown: true,
                    affirmativeText: "Save",
                    negativeText: "Cancel",
                    affirmativeAction: userOAuthPassword =>
                    {
                        if (!string.IsNullOrEmpty(userOAuthPassword))
                        {
                            UserOAuthPassword = userOAuthPassword;
                        }

                        doneAction?.Invoke(true);
                    },
                    negativeAction: null
                )
            );
        }

        /// <summary>
        /// Test the connection to Twitch with provided credentials.
        /// </summary>
        /// <param name="doneAction">Action to make on successful test.</param>
        private void Test(Action successAction)
        {
            try
            {
                InformationManager.DisplayMessage(new InformationMessage("Testing the connection to Twitch, this might take some time..."));

                var credentials = new ConnectionCredentials(UserName, UserOAuthPassword);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };

                var customClient = new WebSocketClient(clientOptions);
                var twitchClient = new TwitchClient(customClient);
                twitchClient.Initialize(credentials, channel: UserName);
                twitchClient.OnConnected += (sender, e) =>
                {
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
                };
                twitchClient.OnConnectionError += (sender, e) =>
                {
                    onError();
                };
                twitchClient.Connect();
            }
            catch
            {
                onError();
            }

            void onError()
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
