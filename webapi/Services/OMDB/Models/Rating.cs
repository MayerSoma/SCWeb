using Newtonsoft.Json;

namespace webapi.Services.OMDB.Models
{
    public class Rating
    {
        [JsonProperty("Source")]
        public string Source { get; set; }
        
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
