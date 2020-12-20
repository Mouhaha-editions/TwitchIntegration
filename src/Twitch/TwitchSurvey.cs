using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitchIntegration.Twitch
{
    public class TwitchSurvey
    {
        public Dictionary<int, string> Choices { get; set; }
        public Dictionary<int, int> Votes { get; set; }
        public DateTime EndDate { get; set; }

        public TwitchSurvey(Dictionary<int, string> choices)
        {
            Choices = choices;
            Votes = choices.ToDictionary(choice => choice.Key, choice => 0);
            EndDate = DateTime.Now.AddSeconds(10);
        }

        public void ProcessAnswer(string text)
        {
            if (int.TryParse(text, out int number))
            {
                if (Votes.ContainsKey(number))
                {
                    Votes[number] += 1;
                }
            }
        }

        public string GetResult()
        {
            var result = default(string);
            var maxVote = 0;

            foreach (var vote in Votes)
            {
                if (maxVote < vote.Value)
                {
                    maxVote = vote.Value;
                    result = string.Concat(vote.Key + " : ", Choices[vote.Key]);
                }
            }

            return result;
        }
    }
}
