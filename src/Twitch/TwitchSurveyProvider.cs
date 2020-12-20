using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchIntegration.Twitch
{
    public class TwitchSurveyProvider
    {
        public TwitchConfiguration TwitchConfiguration { get; set; }
        public TwitchClient TwitchClient { get; set; }
        public TwitchSurvey TwitchSurvey { get; set; }

        public TwitchSurveyProvider(TwitchConfiguration twitchConfiguration)
        {
            TwitchConfiguration = twitchConfiguration;
        }

        public void Connect()
        {
            var credentials = new ConnectionCredentials(TwitchConfiguration.UserName, TwitchConfiguration.UserOAuthPassword);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            TwitchClient = new TwitchClient(new WebSocketClient(clientOptions));
            TwitchClient.Initialize(credentials, channel: TwitchConfiguration.UserName);
            TwitchClient.OnMessageReceived += OnMessageReceived;
            TwitchClient.Connect();
        }

        public void StartSurvey(Dictionary<int, string> choices)
        {
            if (!TwitchClient.IsConnected) return;

            var message = "It's up to you now, choose the answer:";
            foreach (var choice in choices)
            {
                message += string.Concat("\r", choice.Key, " : ", choice.Value);
            }

            TwitchClient.SendMessage(channel: TwitchConfiguration.UserName, message);
            TwitchSurvey = new TwitchSurvey(choices);
        }

        private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            TwitchSurvey?.ProcessAnswer(e.ChatMessage.Message);
        }

        public void Tick()
        {
            if (TwitchSurvey is null) return;
            if (DateTime.Now < TwitchSurvey.EndDate) return;

            var result = TwitchSurvey.GetResult();
            InformationManager.DisplayMessage(new InformationMessage($"Chat has voted : '{result ?? "nothing"}'"));

            TwitchSurvey = null;
        }
    }
}
