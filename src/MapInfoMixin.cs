
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

            // Dans l'idée il faudra récupéréer les options et la conversation avant qu'elle soit affichée.
           // Campaign.Current.ConversationManager.ConversationBegin; semble etre une des clé de ce problème mais je n'ai aucune connaissanence en events .. :/ 


           // ci-dessous ne fonctionne pas, les options ne se remplacent pas dans le texte dans l'idée il faudrait pouvoir modifier le texte toutes 
            // les X secondes en fonction de ce que les viewwers auront dit dans le tchat.
           var CurrentConversation = Campaign.Current.ConversationManager;
            if (CurrentConversation.CurOptions != null)
            {
                var options = new List<ConversationSentenceOption>(CurrentConversation.CurOptions);

                if (options != null && options.Count > 0)
                {
                    CurrentConversation.ClearCurrentOptions();
                    foreach (var option in options)
                    {
                        var textEdited = new TextObject("Test insertion " + option.Text.ToString());
                        //var hintTextEdited = new TextObject("Test insertion hint " + option.HintText.ToString());
                        Campaign.Current.ConversationManager.AddToCurrentOptions(textEdited, option.Id, option.IsClickable, option.HintText);
                    }
                }
            }
        }


    }
}
