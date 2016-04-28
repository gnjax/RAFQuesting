using System;
using System.ServiceModel;

namespace RAFQuesting.Api
{
    [ServiceContract]
    interface ISynchronizationApi {
        [OperationContract]
        bool Initialize(int id);
        [OperationContract(IsOneWay = true)]
        void Heartbeat(int id);
    }
}
