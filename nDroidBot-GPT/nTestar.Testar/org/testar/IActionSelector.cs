using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar
{
    public interface IActionSelector
    {
        Action? selectAction(State state, ISet<Action> actions);
    }
}
