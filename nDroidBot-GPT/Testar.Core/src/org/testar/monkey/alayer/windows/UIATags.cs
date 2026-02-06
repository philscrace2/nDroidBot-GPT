using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public static class UIATags
    {
        public static readonly Tag<string> UIAFrameworkId = Tag<string>.from<string>("UIAFrameworkId", typeof(string));
        public static readonly Tag<bool> UIAIsWindowModal = Tag<bool>.from<bool>("UIAIsWindowModal", typeof(bool));
    }
}
