using Newtonsoft.Json;

namespace PluginDevOps.Models
{
    public class PRResponse
    {
        [JsonProperty("count")]
        public string Count { get; set; }
    }
}
