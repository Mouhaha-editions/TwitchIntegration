﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace TwitchIntegration.Behaviors
{
    public class AddChoicesNumbersBehavior : CampaignBehaviorBase
    {
        public bool Initialized { get; set; }

        public override void RegisterEvents()
        {
            CampaignEvents.SetupPreConversationEvent.AddNonSerializedListener(this, InsertChoicesNumbers);
        }

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

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}