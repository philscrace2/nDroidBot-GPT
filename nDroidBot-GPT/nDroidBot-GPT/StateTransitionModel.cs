using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDroidBot_GPT
{
    public class StateTransitionModel
    {
        private Dictionary<int, State> states = new Dictionary<int, State>();
        private int stateCounter = 0;

        public int AddState(State state)
        {
            stateCounter++;
            states[stateCounter] = state;
            return stateCounter;
        }

        public void AddTransition(int fromStateId, int toStateId, Event transition)
        {
            if (states.ContainsKey(fromStateId))
            {
                states[fromStateId].Transitions.Add(transition);
            }
        }
    }

}
