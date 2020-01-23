using Newtonsoft.Json;

namespace PluginDevOps.Models
{
    public class ReleaseResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("environments")]
        public ReleaseEnvironment[] Environments { get; set; }
    }

    public class ReleaseEnvironment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
