using System;
namespace RAFQuesting.Logic
{
    class Client {
        public enum State {
            QUESTING,
            WAITING,
            TURNEDIN
        }
        public State state { get; set; }
        public int questId { get; set; }
        public DateTime lastHeartbeat { get; set; }

        public Client() {
            state = State.QUESTING;
            lastHeartbeat = DateTime.Now;
        }
    }
}
