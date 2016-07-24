using System.Collections.Generic;

namespace OrComp
{
    public class State
    {
        private int _stateNumber;
        private List<Transition> _transitions;

        public int StateNumber
        {
            get
            {
                return _stateNumber;
            }
        }

        public List<Transition> Transitions
        {
            get
            {
                return _transitions;
            }
        }

        public State(int number)
        {
            _stateNumber = number;
        }

        public void AddTransition(Transition t)
        {
            if (_transitions == null)
                _transitions = new List<Transition>();

            _transitions.Add(t);
        }
    }
}
