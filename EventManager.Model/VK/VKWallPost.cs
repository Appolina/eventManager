using Newtonsoft.Json;

namespace EventManager.Model.VK
{
    internal class VKWallPost
    {


        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "from_id")]
        public int From { get; set; }

        [JsonProperty(PropertyName = "owner_id")]
        public int Owner { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "date")]
        public long Date { get; set; }

    }
}