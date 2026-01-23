namespace org.testar.monkey.alayer
{
    public static class Roles
    {
        public static readonly Role Widget = Role.from("Widget");
        public static readonly Role Invalid = Role.from("Invalid");
        public static readonly Role Control = Role.from("Control", Widget);
        public static readonly Role Expander = Role.from("Expander", Control);
        public static readonly Role Hider = Role.from("Hider", Control);
        public static readonly Role Button = Role.from("Button", Control);
        public static readonly Role StateButton = Role.from("StateButton", Button);
        public static readonly Role ToggleButton = Role.from("ToggleButton", StateButton);
        public static readonly Role Item = Role.from("Item", Control);
        public static readonly Role ItemContainer = Role.from("ItemContainer", Control);
        public static readonly Role Text = Role.from("Text", Control);
        public static readonly Role Decoration = Role.from("Decoration", Control);
        public static readonly Role Slider = Role.from("Slider", Control);
        public static readonly Role Dialog = Role.from("Dialog", Control);
        public static readonly Role MessageBox = Role.from("MessageBox", Dialog);
        public static readonly Role DragSource = Role.from("DragSource", Control);
        public static readonly Role DropTarget = Role.from("DropTarget", Control);
        public static readonly Role SUT = Role.from("SUT");
        public static readonly Role System = Role.from("System", Widget);
        public static readonly Role Process = Role.from("Process", System);
    }
}
