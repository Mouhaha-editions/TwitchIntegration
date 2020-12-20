using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TwitchIntegration.Behaviors;
using TwitchIntegration.Twitch;

namespace TwitchIntegration
{
    public class MySubModule : MBSubModuleBase
    {
        /// <summary>
        /// Twitch configuration.
        /// </summary>
        protected virtual TwitchConfiguration TwitchConfiguration { get; set; }

        /// <summary>
        /// Twitch survey.
        /// </summary>
        protected virtual TwitchSurveyProvider TwitchSurvey { get; set; }

        /// <summary>
        /// Called when the sub module is loaded by the game.
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            TwitchConfiguration = new TwitchConfiguration();
            TwitchSurvey = new TwitchSurveyProvider(TwitchConfiguration);

            Module.CurrentModule.AddInitialStateOption(
                new InitialStateOption("twitch_integration", new TextObject("Twitch integration"), 9990, TwitchConfiguration.Configure, isDisabled: false)
            );
        }

        /// <summary>
        /// Called each time a new game starts, whether it's a new or existing game.
        /// </summary>
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                var campaignStarter = (CampaignGameStarter)gameStarter;

                TwitchSurvey.Connect();

                var dialogChoicesSurveyBehavior = new DialogChoicesBehavior();
                dialogChoicesSurveyBehavior.OnPlayerChoice += TwitchSurvey.StartSurvey;

                campaignStarter.AddBehavior(dialogChoicesSurveyBehavior);
            }
        }

        /// <summary>
        /// Called on each game tick.
        /// </summary>
        protected override void OnApplicationTick(float dt)
        {
            TwitchSurvey.Tick();
        }
    }
}
