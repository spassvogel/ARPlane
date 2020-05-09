using System;
using DarkRift.Server;

namespace ARPlaneDRPlugin
{
    public class ARPlanePlugin : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);

        public ARPlanePlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {

        }
    }
}
