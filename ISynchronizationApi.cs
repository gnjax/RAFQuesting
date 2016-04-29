using System;
using System.ServiceModel;

namespace RAFQuesting.Api
{
    [ServiceContract]
    interface ISynchronizationApi {
        [OperationContract]
        bool initialize(int id);
        [OperationContract/*(IsOneWay = true)*/]
        bool heartbeat(int id);
        [OperationContract]
        bool canTurnIn(int id, int questId);
        [OperationContract]
        bool canContinueQuesting(int id);
    }
}
