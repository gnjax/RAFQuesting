// Gnjax's RAFQuesting plugin
// This plugin is free to use and redistribute, credits appreciated tho
using RAFQuesting.Api;
using Styx;
using Styx.Plugins;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace RAFQuesting.Logic
{
    class RAFQuesting : HBPlugin{
        public override string Author { get { return "Gnjax"; } }
        public override string Name { get { return "RAFQuesting"; } }
        public override Version Version { get { return new Version(2, 0); } }
        private int processId;        
        private bool isMaster;
        private String endpointName;
        private ServiceHost host;

        public RAFQuesting() {
            processId = Process.GetCurrentProcess().Id;
        }

        public override void OnEnable() {
            base.OnEnable();
            if (isMaster) { //Host
                host = new ServiceHost(typeof(SynchronizationApi), new Uri("net.pipe://localhost/RAFQuesting"));
                host.AddServiceEndpoint(typeof(ISynchronizationApi), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue }, endpointName);
                host.Open();
            }
            else { //Consummer

            }
        }

        public override void OnDisable() {
            base.OnDisable();
        }

        public override void Pulse() {

        }
    }
}
