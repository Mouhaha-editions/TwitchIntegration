using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TwitchIntegration.Behaviors;

namespace TwitchIntegration
{
    public class MySubModule : MBSubModuleBase
    {
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
