using System.Collections.Generic;

namespace org.testar.monkey.alayer.actions
{
    public static class BriefActionRolesMap
    {
        public static readonly Dictionary<string, string> map = new()
        {
            { "LeftClickAt", "LC" },
            { "RightClickAt", "RC" },
            { "ClickTypeInto", "T" }
        };
    }
}
