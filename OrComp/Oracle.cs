using System.Collections.Generic;

namespace OrComp
{
    public class Oracle
    {
        private List<State> _states;

        public long NumberOfStates
        {
            get
            {
                return _states.Count;
            }
        }

        public List<State> States
        {
            get
            {
                return _states;
            }
        }

        public Oracle()
        {
            _states = new List<State>();
        }


        public State GetState(int stateNumber)
        {
            if (stateNumber < 0)
                return null;

            return _states[stateNumber];
        }

        public void AddState(int stateNumber)
        {
            State state = new State(stateNumber);
            _states.Add(state);
        }

        public void AddTransition(int from, int to)
        {
            Transition transition = new Transition(from, to, _states);
        }

        public OracleStatistics GetStatistics()
        {
            OracleStatistics stats = new OracleStatistics();

            State state;
            long transSize = 0;
            long stateSize = 0;

            stats.NumberOfStates = NumberOfStates;

            for (int i = 0; i < NumberOfStates; i++)
            {
                state = GetState(i);
                var transitions = state.Transitions;

                if (transitions != null)
                {
                    transSize += transitions.Count;
                }

                stateSize += 96;
            }

            stats.MemoryUsageStates = stateSize;
            stats.MemoryUsageTransitions = transSize * 16;
            stats.TotalMemoryUsage = stateSize + (transSize * 16);

            return stats;
        }
      
        /// <summary>
        /// Returns either the state number of the destination state or -2 if there is no destination state.
        /// </summary>
        /// <param name="stateFrom"></param>
        /// <param name="buffer"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public int FindTransition(State stateFrom, byte[] buffer, char s)
        {
            if (stateFrom != null)
            {
                Transition temp;
                var transitions = stateFrom.Transitions;

                if (transitions != null)
                {
                    for (int i = 0; i < transitions.Count; i++)
                    {
                        temp = transitions[i];
                        if (buffer[temp.To - 1] == (byte)s)
                        {
                            return temp.To;
                        }
                    }
                }

                if (buffer[stateFrom.StateNumber] == (byte)s)
                    return stateFrom.StateNumber + 1;

                return -2;
            }
            if (buffer[0] == (byte)s)
                return 1;

            return -2;
        }
    }
}
