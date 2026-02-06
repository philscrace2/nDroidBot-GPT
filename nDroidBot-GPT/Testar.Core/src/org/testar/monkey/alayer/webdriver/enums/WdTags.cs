using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.webdriver.enums
{
    public static class WdTags
    {
        public static readonly Tag<string> WebHref = Tag<string>.from<string>("WebHref", typeof(string));
        public static readonly Tag<string> WebType = Tag<string>.from<string>("WebType", typeof(string));
    }
}
