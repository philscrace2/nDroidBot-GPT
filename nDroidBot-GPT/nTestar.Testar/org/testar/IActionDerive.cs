using System.Collections.Generic;
using org.testar.monkey.alayer;
using State = org.testar.monkey.alayer.State;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar
{
    public interface IActionDerive
    {
        ISet<Action> deriveActions(ISet<Action> actions);
    }
}
