using Newtonsoft.Json;

namespace PluginDevOps.Models
{
    public class ReleaseListResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("value")]
        public ReleaseDefinition[] Releases { get; set; }
    }

    public class ReleaseDefinition
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
