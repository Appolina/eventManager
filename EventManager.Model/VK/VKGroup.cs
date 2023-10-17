using Newtonsoft.Json;

namespace EventManager.Model.VK
{
    internal class VKGroup
    {
        [JsonProperty(PropertyName = "gid")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "photo_100")]
        public string Photo100 { get; internal set; }
        [JsonProperty(PropertyName = "photo_medium")]
        public string PhotoMegium { get; internal set; }

        public string PhotoUri { get { return Photo100 ?? PhotoMegium; } }
    }
}