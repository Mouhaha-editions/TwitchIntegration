using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace TwitchIntegration.Behaviors
{
    public class DialogChoicesBehavior : CampaignBehaviorBase
    {
        /// <summary>
        /// Whether or not this behavior has been initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        /// <summary>
        /// Fired when the player has to make a choice.
        /// </summary>
        public event Action<Dictionary<int, string>> OnPlayerChoice;

        /// <summary>
        /// Register events.
        /// </summary>
        public override void RegisterEvents()
        {
            CampaignEvents.SetupPreConversationEvent.AddNonSerializedListener(this, InsertChoicesNumbers);
            Campaign.Current.ConversationManager.ConversationBegin += SendPlayerChoices;
            Campaign.Current.ConversationManager.ConversationContinued += SendPlayerChoices;
        }

        /// <summary>
        /// Check every sentences in the game and insert a number in front of each player choices.
        /// </summary>
        protected virtual void InsertChoicesNumbers()
        {
            if (!Initialized)
            {
                var campaign = Campaign.Current;
                var sentencesField = campaign.ConversationManager.GetType().GetField("_sentences", BindingFlags.NonPublic | BindingFlags.Instance);
                if (sentencesField != null)
                {
                    var sentences = sentencesField.GetValue(campaign.ConversationManager) as List<ConversationSentence>;
                    var allPlayerSentences = sentences.Where(s => s.IsPlayer);
                    var allPlayerSentencesCount = allPlayerSentences.Count();
                    var groupedPlayerSentences = allPlayerSentences.GroupBy(s => s.InputToken);
                    foreach (var playerSentences in groupedPlayerSentences)
                    {
                        var i = 1;
                        foreach (var playerSentence in playerSentences)
                        {
                            var textProperty = playerSentence.GetType().GetProperty("Text");
                            if (textProperty != null)
                            {
                                var text = textProperty.GetValue(playerSentence) as TextObject;
                                var valueField = text.GetType().GetField("Value", BindingFlags.NonPublic | BindingFlags.Instance);
                                if (valueField != null)
                                {
                                    var value = (string)valueField.GetValue(text);
                                    textProperty.SetValue(playerSentence, new TextObject($"{i} : {value}"));
                                }
                            }

                            i++;
                        }
                    }
                }

                Initialized = true;
            }
        }

        /// <summary>
        /// Send choices to registered handler.
        /// </summary>
        protected virtual void SendPlayerChoices()
        {
            if (!Campaign.Current.ConversationManager.CurOptions.Any()) return;

            var choices = GetPlayerChoices();
            if (!choices.Any()) return;
            if (choices.Count == 1) return;

            OnPlayerChoice?.Invoke(choices);
        }

        /// <summary>
        /// Get the number of each player choices.
        /// </summary>
        protected virtual Dictionary<int, string> GetPlayerChoices()
        {
            var choices = new Dictionary<int, string>();

            foreach (var choice in Campaign.Current.ConversationManager.CurOptions)
            {
                var textSplitted = choice.Text.ToString().Split(':');
                var numberStr = textSplitted.ElementAtOrDefault(0)?.Trim();
                var rest = string.Join(":", textSplitted.Skip(1).Select(text => text.Trim()));

                if (int.TryParse(numberStr, out int number))
                {
                    choices.Add(number, rest);
                }
            }

            return choices;
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
