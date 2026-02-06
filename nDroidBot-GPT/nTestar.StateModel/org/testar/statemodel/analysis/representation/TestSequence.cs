using System.Linq;

namespace org.testar.statemodel.analysis.representation
{
    public class TestSequence
    {
        public const int VerdictSuccess = 1;
        public const int VerdictInterruptByUser = 2;
        public const int VerdictInterruptBySystem = 3;
        public const int VerdictUnknown = 4;

        public TestSequence(string sequenceId, string startDateTime, string numberOfSteps, int verdict, bool deterministic)
        {
            SequenceId = sequenceId;
            StartDateTime = startDateTime;
            NumberOfSteps = numberOfSteps;
            if (new[] { VerdictSuccess, VerdictInterruptByUser, VerdictInterruptBySystem }.Contains(verdict))
            {
                Verdict = verdict;
            }
            else
            {
                Verdict = VerdictUnknown;
            }
            Deterministic = deterministic;
        }

        public string SequenceId { get; set; }

        public string StartDateTime { get; set; }

        public string NumberOfSteps { get; set; }

        public int Verdict { get; }

        public int NrOfErrors { get; set; }

        public bool Deterministic { get; }

        public string GetVerdictIcon()
        {
            return Verdict switch
            {
                VerdictSuccess => "fa-thumbs-up",
                VerdictInterruptByUser => "fa-hand-paper",
                VerdictInterruptBySystem => "fa-exclamation",
                _ => "fa-question"
            };
        }

        public string GetVerdictTooltip()
        {
            return Verdict switch
            {
                VerdictSuccess => "Succesfully executed.",
                VerdictInterruptByUser => "Execution halted by user.",
                VerdictInterruptBySystem => "Execution halted due to an error.",
                _ => "Unknown result"
            };
        }

        public string GetDeterministicIcon()
        {
            return Deterministic ? "fa-check" : "fa-times";
        }
    }
}
