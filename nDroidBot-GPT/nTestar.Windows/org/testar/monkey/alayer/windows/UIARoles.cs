using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.windows
{
    public static class UIARoles
    {
        public static Role FromControlType(string? controlTypeProgrammaticName)
        {
            if (string.IsNullOrWhiteSpace(controlTypeProgrammaticName))
            {
                return Roles.Control;
            }

            string value = controlTypeProgrammaticName.Trim();

            if (ContainsAny(value, "Button", "SplitButton", "Hyperlink"))
            {
                return Roles.Button;
            }

            if (ContainsAny(value, "CheckBox", "RadioButton", "ToggleButton"))
            {
                return Roles.ToggleButton;
            }

            if (ContainsAny(value, "Edit", "Document", "Text"))
            {
                return Roles.Text;
            }

            if (ContainsAny(value, "ListItem", "TreeItem", "DataItem", "MenuItem", "TabItem"))
            {
                return Roles.Item;
            }

            if (ContainsAny(value, "List", "Tree", "MenuBar", "Menu", "Tab", "ToolBar", "Table"))
            {
                return Roles.ItemContainer;
            }

            if (ContainsAny(value, "Window", "Pane"))
            {
                return Roles.Dialog;
            }

            if (ContainsAny(value, "Slider", "ScrollBar", "Spinner"))
            {
                return Roles.Slider;
            }

            return Roles.Control;
        }

        private static bool ContainsAny(string value, params string[] candidates)
        {
            foreach (string candidate in candidates)
            {
                if (value.Contains(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
