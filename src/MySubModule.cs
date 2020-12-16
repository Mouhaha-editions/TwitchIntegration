using Bannerlord.UIExtenderEx;
using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TwitchIntegration
{
    public class MySubModule : MBSubModuleBase
    {
        private UIExtender _uiExtender = new UIExtender("TwitchIntegration");
        protected override void OnSubModuleLoad()
        {
            try
            {
                base.OnSubModuleLoad();
                _uiExtender.Register(typeof(MySubModule).Assembly);
                _uiExtender.Enable();
            }
            catch (Exception e)
            {
                var ex = e;
            }

        }


        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            try
            {
                base.OnBeforeInitialModuleScreenSetAsRoot();

                
            }
            catch (Exception e)
            {
                var ex = e;
            }
        }
      
    }
}
