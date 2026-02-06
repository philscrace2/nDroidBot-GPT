using org.testar.statemodel.exceptions;

namespace org.testar.statemodel.actionselector
{
    public interface ActionSelector
    {
        void NotifyNewSequence();

        AbstractAction SelectAction(AbstractState currentState, AbstractStateModel abstractStateModel);
    }
}
