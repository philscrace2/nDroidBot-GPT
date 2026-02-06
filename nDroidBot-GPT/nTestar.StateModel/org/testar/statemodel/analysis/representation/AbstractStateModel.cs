using System.Collections.Generic;
using System.Linq;

namespace org.testar.statemodel.analysis.representation
{
    public class AbstractStateModel
    {
        public AbstractStateModel(string applicationName, string applicationVersion, string modelIdentifier, ISet<string> abstractionAttributes, List<TestSequence> sequences)
        {
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            ModelIdentifier = modelIdentifier;
            AbstractionAttributes = abstractionAttributes;
            Sequences = sequences;
        }

        public string ApplicationVersion { get; set; }

        public string ApplicationName { get; set; }

        public string ModelIdentifier { get; set; }

        public ISet<string> AbstractionAttributes { get; set; }

        public List<TestSequence> Sequences { get; set; }

        public string GetAbstractionAttributesAsString()
        {
            return string.Join(", ", AbstractionAttributes.OrderBy(value => value));
        }
    }
}
