using System;
using System.ServiceModel;

namespace RAFQuesting.Api
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    class SynchronizationApi {
        bool initialize(int id) {
            return true;
        }
        void heartbeat(int id) {

        }
    }
}
