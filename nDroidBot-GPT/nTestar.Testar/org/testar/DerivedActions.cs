namespace org.testar
{
    public class DerivedActions
    {
        private readonly System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> availableActions;
        private readonly System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> filteredActions;

        public DerivedActions(System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> availableActions,
            System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> filteredActions)
        {
            this.availableActions = availableActions;
            this.filteredActions = filteredActions;
        }

        public System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> getAvailableActions()
        {
            return availableActions;
        }

        public System.Collections.Generic.ISet<org.testar.monkey.alayer.Action> getFilteredActions()
        {
            return filteredActions;
        }

        public void addAvailableAction(org.testar.monkey.alayer.Action action)
        {
            availableActions.Add(action);
        }

        public void addFilteredAction(org.testar.monkey.alayer.Action action)
        {
            filteredActions.Add(action);
        }
    }
}
