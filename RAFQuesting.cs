// Gnjax's RAFQuesting plugin
// This plugin is free to use and redistribute, credits appreciated tho
using RAFQuesting.Api;
using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.CommonBot.Profiles;
using Styx.Plugins;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Collections.Generic;


namespace RAFQuesting.Logic
{
    class RAFQuestingMain : HBPlugin{
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public override string Author { get { return "Gnjax"; } }
        public override string Name { get { return "RAFQuesting"; } }
        public override Version Version { get { return new Version(2, 0); } }
        private ISynchronizationApi syncApi { get; set; }
        private static ChannelFactory<ISynchronizationApi> channelFactory;
        private int processId;        
        private bool isMaster;
        private string endpointName;
        private string currentProfile;
        private ServiceHost host;
        private Client.State myState;
        static public Dictionary<int, Client> clients;

        public RAFQuestingMain() {
            processId = Process.GetCurrentProcess().Id;
            myState = Client.State.QUESTING;
        }

        public override void OnEnable() {
            base.OnEnable();
            if (isMaster) {
                host = new ServiceHost(typeof(SynchronizationApi), new Uri("net.pipe://localhost/RAFQuesting"));
                host.AddServiceEndpoint(typeof(ISynchronizationApi), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue }, endpointName);
                host.Open();
            }
            channelFactory = new ChannelFactory<ISynchronizationApi>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/RAFQuesting/" + endpointName));
            syncApi = channelFactory.CreateChannel();
            syncApi.initialize(processId);
        }

        public override void OnDisable() {
            base.OnDisable();
            if (isMaster) {
                if (host.State == CommunicationState.Opened || host.State == CommunicationState.Opening) {
                    host.Close();
                    host.Abort();
                }
            }
            if (channelFactory.State == CommunicationState.Opened || channelFactory.State == CommunicationState.Opening) {
                channelFactory.Close();
                channelFactory.Abort();
            }
        }

        public override void Pulse() {
            syncApi.heartbeat(processId); //TODO: Do something in case false is returned
            if (myState == Client.State.QUESTING) {
                if (Me.CurrentTarget != null && Me.CurrentTarget.QuestGiverStatus == QuestGiverStatus.TurnIn) {
                    if (Me.CurrentTarget.WithinInteractRange) {
                        WoWMovement.MoveStop();
                        currentProfile = ProfileManager.XmlLocation;
                        ProfileManager.LoadEmpty();
                        myState = Client.State.WAITING;
                    }  
                }
            }
            else if (myState == Client.State.WAITING) {
                if (syncApi.canTurnIn(processId, 0)) {
                    ProfileManager.LoadNew(currentProfile);
                    myState = Client.State.TURNEDIN;
                }
            }
            else {
                if (!syncApi.canContinueQuesting(processId)) {
                    //Find a way to check if the quest has been turned in yet
                    WoWMovement.MoveStop();
                    currentProfile = ProfileManager.XmlLocation;
                    ProfileManager.LoadEmpty();
                }
                else {
                    ProfileManager.LoadNew(currentProfile);
                    myState = Client.State.QUESTING;
                }
            }
        }
    }
}
