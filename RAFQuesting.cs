// Gnjax's RAFQuesting plugin
// This plugin is free to use and redistribute, credits appreciated tho
using RAFQuesting.Api;
using Styx;
using Styx.Plugins;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Collections.Generic;

namespace RAFQuesting.Logic
{
    class RAFQuestingMain : HBPlugin{
        public override string Author { get { return "Gnjax"; } }
        public override string Name { get { return "RAFQuesting"; } }
        public override Version Version { get { return new Version(2, 0); } }
        private ISynchronizationApi SyncApi { get; set; }
        private static ChannelFactory<ISynchronizationApi> channelFactory;
        private int processId;        
        private bool isMaster;
        private String endpointName;
        private ServiceHost host;
        static public Dictionary<int, Client> clients;

        public RAFQuestingMain() {
            processId = Process.GetCurrentProcess().Id;
        }

        public override void OnEnable() {
            base.OnEnable();
            if (isMaster) {
                host = new ServiceHost(typeof(SynchronizationApi), new Uri("net.pipe://localhost/RAFQuesting"));
                host.AddServiceEndpoint(typeof(ISynchronizationApi), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue }, endpointName);
                host.Open();
            }
            else {
                channelFactory = new ChannelFactory<ISynchronizationApi>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/RAFQuesting/" + endpointName));
                SyncApi = channelFactory.CreateChannel();
                SyncApi.initialize(processId);
            }
        }

        public override void OnDisable() {
            base.OnDisable();
            if (isMaster) {
                if (host.State == CommunicationState.Opened || host.State == CommunicationState.Opening) {
                    host.Close();
                    host.Abort();
                }
            }
            else {
                if (channelFactory.State == CommunicationState.Opened || channelFactory.State == CommunicationState.Opening) {
                    channelFactory.Close();
                    channelFactory.Abort();
                }
            }
        }

        public override void Pulse() {
            if (isMaster) {

            }
            else {

            }
        }
    }
}
