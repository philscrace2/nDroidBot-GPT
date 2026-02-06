using System.Collections.Generic;
using org.testar.monkey.alayer;

namespace org.testar
{
    public interface IActionDerive
    {
        ISet<Action> deriveActions(ISet<Action> actions);
    }
}
