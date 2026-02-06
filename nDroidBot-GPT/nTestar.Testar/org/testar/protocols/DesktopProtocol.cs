using System.Collections.Generic;
using org.testar;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.protocols
{
    public class DesktopProtocol : GenericUtilsProtocol
    {
        protected virtual DerivedActions deriveClickTypeScrollActionsFromAllWidgets(ISet<Action> actions, State state)
        {
            return new DerivedActions(actions, new HashSet<Action>());
        }

        protected virtual DerivedActions deriveClickTypeScrollActionsFromTopLevelWidgets(ISet<Action> actions, State state)
        {
            return new DerivedActions(actions, new HashSet<Action>());
        }
    }
}
