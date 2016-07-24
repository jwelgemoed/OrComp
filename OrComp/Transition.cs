using System.Collections.Generic;

namespace OrComp
{
    public class Transition
    {
        private int _to;
        private int _from;

        public int From
        {
            get
            {
                return _from;
            }
        }

        public int To
        {
            get
            {
                return _to;
            }
        }

        public Transition(int from, int to, List<State> states)
        {
            _from = from;
            _to = to;
            State tempState = states[from];
            tempState.AddTransition(this);
        }
    }
}
