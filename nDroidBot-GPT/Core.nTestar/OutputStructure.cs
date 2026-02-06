
namespace Core.nTestar
{
    internal class OutputStructure
    {
        internal static int SequenceInnerLoopCount
        {
            get => org.testar.OutputStructure.sequenceInnerLoopCount;
            set => org.testar.OutputStructure.sequenceInnerLoopCount = value;
        }

        internal static void CalculateInnerLoopDateString()
        {
            org.testar.OutputStructure.calculateInnerLoopDateString();
        }
    }
}
