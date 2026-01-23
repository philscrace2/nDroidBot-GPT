using org.testar.monkey.alayer;

namespace org.testar.monkey.alayer.actions
{
    public static class ActionRoles
    {
        public static readonly Role Action = Role.from("Action");
        public static readonly Role NOPAction = Role.from("NOPAction", Action);
        public static readonly Role MouseAction = Role.from("MouseAction", Action);
        public static readonly Role KeyboardAction = Role.from("KeyboardAction", Action);
        public static readonly Role CompoundAction = Role.from("CompoundAction", Action);
        public static readonly Role MouseMove = Role.from("MouseMove", MouseAction);
        public static readonly Role MouseDown = Role.from("MouseDown", MouseAction);
        public static readonly Role KeyDown = Role.from("KeyDown", KeyboardAction);
        public static readonly Role MouseUp = Role.from("MouseUp", MouseAction);
        public static readonly Role KeyUp = Role.from("KeyUp", KeyboardAction);
        public static readonly Role HitKey = Role.from("HitKey", KeyDown, KeyUp);
        public static readonly Role HitESC = Role.from("HitESC", HitKey);
        public static readonly Role HitShortcutKey = Role.from("HitShortcutKey", KeyDown, KeyUp);
        public static readonly Role Click = Role.from("Click", MouseDown, MouseUp);
        public static readonly Role LeftClick = Role.from("LeftClick", Click);
        public static readonly Role RightClick = Role.from("RightClick", Click);
        public static readonly Role DoubleClick = Role.from("DoubleClick", Click);
        public static readonly Role LDoubleClick = Role.from("LDoubleClick", LeftClick, DoubleClick);
        public static readonly Role RDoubleClick = Role.from("RDoubleClick", RightClick, DoubleClick);
        public static readonly Role ClickAt = Role.from("ClickAt", Click, MouseMove);
        public static readonly Role LeftClickAt = Role.from("LeftClickAt", ClickAt, LeftClick);
        public static readonly Role RightClickAt = Role.from("RightClickAt", ClickAt, RightClick);
        public static readonly Role DoubleClickAt = Role.from("DoubleClickAt", ClickAt, DoubleClick);
        public static readonly Role LDoubleClickAt = Role.from("LDoubleClickAt", DoubleClickAt, LeftClick);
        public static readonly Role RDoubleClickAt = Role.from("RDoubleClickAt", DoubleClickAt, RightClick);
        public static readonly Role Type = Role.from("Type", HitKey);
        public static readonly Role Paste = Role.from("Paste", HitKey);
        public static readonly Role ClickTypeInto = Role.from("ClickTypeInto", ClickAt, Type);
        public static readonly Role DropDown = Role.from("DropDown", Click, KeyDown);
        public static readonly Role PasteTextInto = Role.from("PasteTextInto", ClickAt, Paste);
        public static readonly Role Drag = Role.from("Drag", MouseDown, MouseUp, MouseMove);
        public static readonly Role LeftDrag = Role.from("LeftDrag", Drag);
    }
}
