using Newtonsoft.Json;

namespace Backend.Models{
    public class Provinces{
        [JsonProperty("txn_date")]
        public string date{get; set;}

        [JsonProperty("province")]
        public string province{get; set;}

        [JsonProperty("new_case")]
        public string newCase{get; set;}

        [JsonProperty("total_case")]
        public string totalCase{get; set;}

        [JsonProperty("update_date")]
        public string updateDate{get; set;}
    }
}