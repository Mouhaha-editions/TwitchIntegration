
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.MountAndBlade;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using TaleWorlds.Engine.Screens;
using Bannerlord.UIExtenderEx.ViewModels;
using Bannerlord.UIExtenderEx.Attributes;

namespace TwitchIntegration
{
    [ViewModelMixin("Refresh")]
    public class MapInfoMixin : BaseViewModelMixin<MapInfoVM>
    {
        public MapInfoMixin(MapInfoVM vm) : base(vm)
        { }

        private int _pointsCounter;

        private string _pointsTooltip = "0%";
        //private string _pointsTooltip;

        public string ApiKey { get; set; }

        [DataSourceProperty] public BasicTooltipViewModel CritFailHint => new BasicTooltipViewModel(() => _pointsTooltip);
        [DataSourceProperty] public string CritFailChanceText => "100%";

        public event EventHandler ConversationEvt;

        public override void OnRefresh()
        {
            
               Campaign.Current.ConversationManager.OnConversationActivate();


            var x = Campaign.Current.ConversationManager;
           

           

        }

    }
}
