using Newtonsoft.Json;

namespace EventManager.Model.VK
{
    internal class VKMember
    {
        [JsonProperty(PropertyName = "uid")]
        public int Id { get; private set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; private set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; private set; }


        [JsonProperty(PropertyName = "photo_100")]
        public string Photo { get; private set; }
    }
}