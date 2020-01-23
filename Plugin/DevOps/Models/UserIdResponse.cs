using Newtonsoft.Json;

namespace PluginDevOps.Models
{
    public class UserIdResponse
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonProperty("id")]
        public string UserId { get; set; }
    }
}
