using System.Text.Json.Serialization;

namespace org.testar.statemodel.analysis.jsonformat
{
    public class Edge : Document
    {
        public Edge(string id, string sourceId, string targetId)
            : base(id)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }

        [JsonPropertyName("source")]
        public string SourceId { get; }

        [JsonPropertyName("target")]
        public string TargetId { get; }
    }
}
