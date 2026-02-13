namespace org.testar.monkey.alayer.windows
{
    public static class AccessBridgeControlTypes
    {
        public const int MissingUia = -1;

        public const string Alert = "alert";
        public const string Button = "push button";
        public const string ToggleButton = "toggle button";
        public const string CheckBox = "check box";
        public const string RadioButton = "radio button";
        public const string Text = "text";
        public const string Edit = "edit";
        public const string List = "list";
        public const string ListItem = "list item";
        public const string Tree = "tree";
        public const string TreeItem = "tree item";
        public const string Menu = "menu";
        public const string MenuItem = "menu item";
        public const string Window = "window";
        public const string Dialog = "dialog";
        public const string ComboBox = "combo box";
        public const string ToolBar = "tool bar";
        public const string Slider = "slider";

        private static readonly IReadOnlyDictionary<string, int> RoleToControlTypeId =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                [Alert] = Windows.UIA_TextControlTypeId,
                [Button] = Windows.UIA_ButtonControlTypeId,
                [ToggleButton] = Windows.UIA_ButtonControlTypeId,
                [CheckBox] = Windows.UIA_CheckBoxControlTypeId,
                [RadioButton] = Windows.UIA_RadioButtonControlTypeId,
                [Text] = Windows.UIA_TextControlTypeId,
                [Edit] = Windows.UIA_EditControlTypeId,
                [List] = Windows.UIA_ListControlTypeId,
                [ListItem] = Windows.UIA_ListItemControlTypeId,
                [Tree] = Windows.UIA_TreeControlTypeId,
                [TreeItem] = Windows.UIA_TreeItemControlTypeId,
                [Menu] = Windows.UIA_MenuControlTypeId,
                [MenuItem] = Windows.UIA_MenuItemControlTypeId,
                [Window] = Windows.UIA_WindowControlTypeId,
                [Dialog] = Windows.UIA_WindowControlTypeId,
                [ComboBox] = Windows.UIA_ComboBoxControlTypeId,
                [ToolBar] = Windows.UIA_ToolBarControlTypeId,
                [Slider] = Windows.UIA_SliderControlTypeId
            };

        public static bool TryGetControlTypeId(string? accessibleRole, out int uiaControlTypeId)
        {
            if (string.IsNullOrWhiteSpace(accessibleRole))
            {
                uiaControlTypeId = MissingUia;
                return false;
            }

            if (RoleToControlTypeId.TryGetValue(accessibleRole.Trim(), out int mapped))
            {
                uiaControlTypeId = mapped;
                return true;
            }

            uiaControlTypeId = MissingUia;
            return false;
        }
    }
}
