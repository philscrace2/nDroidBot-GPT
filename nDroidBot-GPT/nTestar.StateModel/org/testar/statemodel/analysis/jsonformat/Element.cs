using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace org.testar.statemodel.analysis.jsonformat
{
    public class Element
    {
        public const string GroupNodes = "nodes";
        public const string GroupEdges = "edges";

        private List<string>? _classes;

        public Element(string group, Document document, string? className = null)
        {
            Group = group;
            Document = document;
            if (className != null)
            {
                _classes = new List<string> { className };
            }
        }

        [JsonPropertyName("group")]
        public string Group { get; }

        [JsonPropertyName("data")]
        public Document Document { get; }

        [JsonPropertyName("classes")]
        public List<string>? Classes => _classes;

        public void AddClass(string className)
        {
            if (_classes == null)
            {
                _classes = new List<string>();
            }
            _classes.Add(className);
        }
    }
}
