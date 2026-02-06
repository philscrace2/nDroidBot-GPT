using System.Collections.Generic;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar
{
    public interface IActionSelector
    {
        Action? selectAction(State state, ISet<Action> actions);
    }
}
