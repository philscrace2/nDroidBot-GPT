namespace org.testar.statemodel.event
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
