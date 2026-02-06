using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace org.testar.statemodel.analysis.jsonformat
{
    public abstract class Document
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        protected Document(string id)
        {
            Id = id;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonExtensionData]
        public Dictionary<string, string> Properties => _properties;

        public void AddProperty(string key, string value)
        {
            _properties[key] = value;
        }
    }
}
