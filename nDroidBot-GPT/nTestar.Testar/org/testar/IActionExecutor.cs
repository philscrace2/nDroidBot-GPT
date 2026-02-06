using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar
{
    public interface IActionExecutor
    {
        void executeAction(Action action);
    }
}
