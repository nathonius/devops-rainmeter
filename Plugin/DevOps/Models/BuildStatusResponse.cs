using Newtonsoft.Json;

namespace PluginDevOps.Models
{
    public class BuildStatusResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("value")]
        public BuildStatus[] Builds { get; set; }
    }

    public class BuildStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("definition")]
        public BuildDefinition Definition { get; set; }
    }

    public class BuildDefinition
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
