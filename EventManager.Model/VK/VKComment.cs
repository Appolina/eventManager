using Newtonsoft.Json;

namespace EventManager.Model.VK
{
    internal class VKComment
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "from_id")]
        public int FromId { get; private set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; private set; }

        [JsonProperty(PropertyName = "date")]
        public long Date { get; private set; }
    }
}