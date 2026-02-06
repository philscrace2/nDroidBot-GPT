namespace org.testar.statemodel.events
{
    public enum StateModelEventType
    {
        AbstractStateAdded,
        AbstractStateChanged,
        AbstractActionChanged,
        AbstractStateTransitionAdded,
        AbstractStateModelInitialized,
        AbstractStateTransitionChanged,
        SequenceStarted,
        SequenceManagerInitialized,
        SequenceNodeAdded,
        SequenceStepAdded,
        SequenceNodeUpdated,
        SequenceEnded
    }
}
