using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static Core.nTestar.RuntimeControlsProtocol;
//using Core.nTestar.RuntimeControlsProtocol;

namespace Core.nTestar
{
    public class GenerateMode
    {
        private bool exceptionThrown = false;

        /// <summary>
        /// Method to run TESTAR in Generate mode.
        /// </summary>
        /// <param name="protocol"></param>
        public void RunGenerateOuterLoop(DefaultProtocol protocol)
        {
            // Method for defining other init actions, like setting up the external environment
            protocol.InitTestSession();

            // Initializing TESTAR for generate mode
            protocol.InitGenerateMode();

            /*
             ***** OUTER LOOP - STARTING A NEW SEQUENCE
             */
            while (protocol.Mode != Modes.Quit && protocol.MoreSequences())
            {
                exceptionThrown = false;

                // Prepare the output folder structure
                lock (this)
                {
                    OutputStructure.CalculateInnerLoopDateString();
                    OutputStructure.SequenceInnerLoopCount++;
                }

                // Empty method in DefaultProtocol - allowing implementation in application-specific protocols
                // HTML report is created here in DefaultProtocol
                protocol.PreSequencePreparations();

                // Reset the faulty variable because we started a new sequence
                DefaultProtocol.FaultySequence = false;

                // Starting system or connecting to a running one
                SUT system = protocol.StartSUTandLogger();

                // Generating the sequence file that can be replayed
                protocol.GetAndStoreGeneratedSequence();
                protocol.GetAndStoreSequenceFile();

                // Initializing TESTAR and the protocol canvas for a new sequence
                protocol.StartTestSequence(system);

                try
                {
                    // getState() called before beginSequence:
                    Console.WriteLine("Obtaining system state before beginSequence...");
                    State state = protocol.GetState(system);

                    // beginSequence() - a script to interact with GUI, for example, login screen
                    Console.WriteLine($"Starting sequence {protocol.SequenceCount} (output as: {protocol.GeneratedSequence})\n");
                    protocol.BeginSequence(system, state);

                    // Update state after begin sequence SUT modification
                    state = protocol.GetState(system);

                    // Notify the state model manager
                    protocol.StateModelManager.NotifyTestSequencedStarted();

                    /*
                     ***** INNER LOOP:
                     */
                    Verdict stateVerdict = RunGenerateInnerLoop(protocol, system, state);
                    protocol.FinalVerdict = stateVerdict.Join(DefaultProtocol.ProcessVerdict);

                    // Calling FinishSequence() to allow scripting GUI interactions to close the SUT
                    protocol.FinishSequence();

                    // Notify the state model manager of the sequence end
                    protocol.StateModelManager.NotifyTestSequenceStopped();

                    protocol.WriteAndCloseFragmentForReplayableSequence();

                    if (DefaultProtocol.FaultySequence)
                        Console.WriteLine("Sequence contained faults!");

                    // Copy sequence file into the proper directory
                    protocol.ClassifyAndCopySequenceIntoAppropriateDirectory(protocol.FinalVerdict, protocol.GeneratedSequence, protocol.CurrentSeq);

                    // Calling postSequenceProcessing() to allow resetting the test environment after test sequence
                    protocol.PostSequenceProcessing();

                    // Ending the test sequence of TESTAR
                    protocol.EndTestSequence();

                    Console.WriteLine("End of test sequence - shutting down the SUT...");
                    protocol.StopSystem(system);
                    Console.WriteLine("... SUT has been shut down!");

                    protocol.SequenceCount++;
                }
                catch (Exception e)
                {
                    string message = $"Thread: name={Thread.CurrentThread.Name}, id={Thread.CurrentThread.ManagedThreadId}, TESTAR throws exception";
                    Console.WriteLine(message);

                    var stackTrace = new StringBuilder();
                    stackTrace.AppendLine(message);
                    stackTrace.AppendLine(e.ToString());

                    protocol.StateModelManager.NotifyTestSequenceInterruptedBySystem(stackTrace.ToString());
                    exceptionThrown = true;
                    Console.WriteLine(e);
                    protocol.EmergencyTerminateTestSequence(system, e);
                }
            }

            if (protocol.Mode == Modes.Quit && !exceptionThrown)
            {
                // The user initiated the shutdown
                protocol.StateModelManager.NotifyTestSequenceInterruptedByUser();
            }

            // Notify the state model manager that the testing has finished
            protocol.StateModelManager.NotifyTestingEnded();

            protocol.Mode = Modes.Quit;
        }

        /// <summary>
        /// This is the inner loop of TESTAR Generate-mode:
        /// - GetState
        /// - GetVerdict
        /// - StopCriteria (MoreActions/MoreSequences/Time?)
        /// - DeriveActions
        /// - SelectAction
        /// - ExecuteAction
        /// </summary>
        private Verdict RunGenerateInnerLoop(DefaultProtocol protocol, SUT system, State state)
        {
            /*
             ***** INNER LOOP:
             */
            while (protocol.Mode != Modes.Quit && protocol.MoreActions(state))
            {
                // getState() including getVerdict() that is saved into the state
                Console.WriteLine("Obtained system state in inner loop of TESTAR...");
                protocol.Canvas.Begin();
                Util.Clear(protocol.Canvas);

                // Deriving actions from the state
                HashSet<Action> actions = protocol.DeriveActions(system, state);
                protocol.BuildStateActionsIdentifiers(state, actions);

                // First check if we have some pre-select action to execute (retryDeriveAction or ESC)
                actions = protocol.PreSelectAction(system, state, actions);

                // Notify the state model manager of the current state
                protocol.StateModelManager.NotifyNewStateReached(state, actions);

                // Showing the green dots if visualization is on
                if (protocol.VisualizationOn)
                {
                    protocol.VisualizeActions(protocol.Canvas, state, actions);
                }

                // Selecting one of the available actions
                Action action = protocol.SelectAction(state, actions);

                // Showing the red dot if visualization is on
                if (protocol.VisualizationOn)
                {
                    SutVisualization.VisualizeSelectedAction(protocol.Settings, protocol.Canvas, state, action);
                }

                // Before action execution, pass it to the state model manager
                protocol.StateModelManager.NotifyActionExecution(action);

                // Executing the selected action
                protocol.ExecuteAction(system, state, action);
                DefaultProtocol.LastExecutedAction = action;
                protocol.ActionCount++;

                // Resetting the visualization
                Util.Clear(protocol.Canvas);
                protocol.Canvas.End();

                // Saving the actions and the executed action into replayable test sequence
                protocol.SaveActionIntoFragmentForReplayableSequence(action, state, actions);

                // Fetch the new state
                state = protocol.GetState(system);
            }

            // Notify the state model manager of the last state
            HashSet<Action> finalActions = protocol.DeriveActions(system, state);
            protocol.BuildStateActionsIdentifiers(state, finalActions);
            protocol.StateModelManager.NotifyNewStateReached(state, finalActions);

            return protocol.GetVerdict(state);
        }
    }

}
