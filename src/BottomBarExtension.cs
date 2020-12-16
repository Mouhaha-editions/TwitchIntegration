
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaleWorlds.Engine;
using Path = System.IO.Path;

namespace TwitchIntegration
{
    internal static class XmlPathHelper
    {
        public static string GetXmlPath(string id) => Path.Combine(Utilities.GetBasePath(), "Modules", "TwitchIntegration", "GUI", "PrefabExtensions", $"{id}.xml");
    }

    [PrefabExtension("MapBar", "descendant::ListPanel[@Id='BottomInfoBar']/Children")]
    public class BottomBarExtension : PrefabExtensionInsertPatch
    {
        public BottomBarExtension()
        {
            var prefab = XmlPathHelper.GetXmlPath(Id);
            try
            {
                using (XmlReader reader = XmlReader.Create(prefab, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true }))
                {
                    XmlDocument.Load(reader);
                }
            }
            catch (Exception e)
            {
                var i = e;
            }
        }

        public override string Id => "HcPointCounter";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;

        //public override string Name => "DayCounterIndicator";
    }
}
