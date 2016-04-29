using System;
using System.ServiceModel;
using System.Collections.Generic;
using RAFQuesting.Logic;

namespace RAFQuesting.Api
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    class SynchronizationApi : ISynchronizationApi {
        public bool initialize(int id) {
            if (RAFQuestingMain.clients.ContainsKey(id)) //Something went wrong, we already have this HB pid registered
                return false;
            Client client = new Client();
            RAFQuestingMain.clients.Add(id, client);
            return true;
        }

        public bool heartbeat(int id) {
            RAFQuestingMain.clients[id].lastHeartbeat = DateTime.Now;
            foreach (KeyValuePair<int, Client> entry in RAFQuestingMain.clients) {  
                DateTime timeout = new DateTime(entry.Value.lastHeartbeat.ToBinary()).AddMinutes(1);
                if (DateTime.Compare(DateTime.Now, timeout) > 0)
                    return false; //A client stopped heartbeating more than a minute ago
            }
            return true;
        }

        public bool canTurnIn(int id, int questId) { //TODO: questId check, probably return int with different state in case of Class/Race specific quest
            RAFQuestingMain.clients[id].questId = questId;
            RAFQuestingMain.clients[id].state = Client.State.WAITING;
            foreach (KeyValuePair<int, Client> entry in RAFQuestingMain.clients) {
                if (entry.Value.state == Client.State.QUESTING)
                    return false;
            }
            RAFQuestingMain.clients[id].state = Client.State.TURNEDIN;
            return true;
        }

        public bool canContinueQuesting(int id) {
            foreach (KeyValuePair<int, Client> entry in RAFQuestingMain.clients) {
                if (entry.Value.state == Client.State.WAITING)
                    return false;
            }
            RAFQuestingMain.clients[id].state = Client.State.QUESTING;
            return true;
        }
    }
}
