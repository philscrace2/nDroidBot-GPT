using org.testar.monkey.alayer;

namespace org.testar
{
    public class ActionStatus
    {
        private Action? action;
        private bool actionSucceeded;
        private bool problems;
        private bool userEventAction;

        public ActionStatus()
        {
            action = null;
            actionSucceeded = true;
            problems = false;
            userEventAction = false;
        }

        public Action? getAction()
        {
            return action;
        }

        public void setAction(Action action)
        {
            this.action = action;
        }

        public bool isActionSucceeded()
        {
            return actionSucceeded;
        }

        public bool setActionSucceeded(bool actionSucceeded)
        {
            this.actionSucceeded = actionSucceeded;
            return actionSucceeded;
        }

        public bool isProblems()
        {
            return problems;
        }

        public void setProblems(bool problems)
        {
            this.problems = problems;
        }

        public bool isUserEventAction()
        {
            return userEventAction;
        }

        public void setUserEventAction(bool userEventAction)
        {
            this.userEventAction = userEventAction;
        }
    }
}
