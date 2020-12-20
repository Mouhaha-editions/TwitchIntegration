using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TwitchIntegration.Behaviors;

namespace TwitchIntegration
{
    public class MySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddInitialStateOption(
                new InitialStateOption("twitch_integration", new TextObject("Twitch integration"), 9990, TwitchManager.Configure, isDisabled: false)
            );
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                var campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new AddChoicesNumbersBehavior());
            }
        }
    }
}
