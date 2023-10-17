using Newtonsoft.Json;

namespace EventManager.Model.VK
{
    internal class CaptchaNeddedError
    {
        [JsonProperty(PropertyName = "error_code")]
        public int Code { get; private set; }

        [JsonProperty(PropertyName = "captcha_sid")]
        public string CaptchaSid { get; private set; }

        [JsonProperty(PropertyName = "captcha_img")]
        public string CaptchaImg { get; private set; }

        [JsonProperty(PropertyName = "error_msg")]
        public string Message { get; private set; }
    }
}
