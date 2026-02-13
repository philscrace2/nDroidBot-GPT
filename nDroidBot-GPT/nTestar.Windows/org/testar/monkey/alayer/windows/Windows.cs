namespace org.testar.monkey.alayer.windows
{
    public static class Windows
    {
        public const int UIA_ButtonControlTypeId = 50000;
        public const int UIA_CalendarControlTypeId = 50001;
        public const int UIA_CheckBoxControlTypeId = 50002;
        public const int UIA_ComboBoxControlTypeId = 50003;
        public const int UIA_EditControlTypeId = 50004;
        public const int UIA_HyperlinkControlTypeId = 50005;
        public const int UIA_ImageControlTypeId = 50006;
        public const int UIA_ListItemControlTypeId = 50007;
        public const int UIA_ListControlTypeId = 50008;
        public const int UIA_TextControlTypeId = 50020;
        public const int UIA_MenuBarControlTypeId = 50010;
        public const int UIA_MenuControlTypeId = 50009;
        public const int UIA_MenuItemControlTypeId = 50011;
        public const int UIA_ProgressBarControlTypeId = 50012;
        public const int UIA_RadioButtonControlTypeId = 50013;
        public const int UIA_ScrollBarControlTypeId = 50014;
        public const int UIA_SliderControlTypeId = 50015;
        public const int UIA_SpinnerControlTypeId = 50016;
        public const int UIA_StatusBarControlTypeId = 50017;
        public const int UIA_TabControlTypeId = 50018;
        public const int UIA_TabItemControlTypeId = 50019;
        public const int UIA_ToolBarControlTypeId = 50021;
        public const int UIA_ToolTipControlTypeId = 50022;
        public const int UIA_TreeControlTypeId = 50023;
        public const int UIA_TreeItemControlTypeId = 50024;
        public const int UIA_CustomControlTypeId = 50025;
        public const int UIA_GroupControlTypeId = 50026;
        public const int UIA_ThumbControlTypeId = 50027;
        public const int UIA_DataGridControlTypeId = 50028;
        public const int UIA_DataItemControlTypeId = 50029;
        public const int UIA_DocumentControlTypeId = 50030;
        public const int UIA_SplitButtonControlTypeId = 50031;
        public const int UIA_WindowControlTypeId = 50032;
        public const int UIA_PaneControlTypeId = 50033;
        public const int UIA_HeaderControlTypeId = 50034;
        public const int UIA_HeaderItemControlTypeId = 50035;
        public const int UIA_TableControlTypeId = 50036;
        public const int UIA_TitleBarControlTypeId = 50037;
        public const int UIA_SeparatorControlTypeId = 50038;
        public const int UIA_SemanticZoomControlTypeId = 50039;
        public const int UIA_AppBarControlTypeId = 50040;

        public const int UIA_IsScrollPatternAvailablePropertyId = 30030;
        public const int UIA_IsSelectionPatternAvailablePropertyId = 30031;
        public const int UIA_IsSelectionItemPatternAvailablePropertyId = 30032;
        public const int UIA_IsTogglePatternAvailablePropertyId = 30034;
        public const int UIA_IsValuePatternAvailablePropertyId = 30043;
        public const int UIA_IsWindowPatternAvailablePropertyId = 30044;

        public const int UIA_ValueValuePropertyId = 30045;
        public const int UIA_ValueIsReadOnlyPropertyId = 30046;

        public const int UIA_SelectionSelectionPropertyId = 30059;
        public const int UIA_SelectionCanSelectMultiplePropertyId = 30060;
        public const int UIA_SelectionIsSelectionRequiredPropertyId = 30061;
        public const int UIA_SelectionItemIsSelectedPropertyId = 30079;
        public const int UIA_SelectionItemSelectionContainerPropertyId = 30080;

        public const int UIA_ScrollHorizontallyScrollablePropertyId = 30088;
        public const int UIA_ScrollHorizontalScrollPercentPropertyId = 30089;
        public const int UIA_ScrollHorizontalViewSizePropertyId = 30090;
        public const int UIA_ScrollVerticallyScrollablePropertyId = 30091;
        public const int UIA_ScrollVerticalScrollPercentPropertyId = 30092;
        public const int UIA_ScrollVerticalViewSizePropertyId = 30093;

        public const int UIA_WindowCanMaximizePropertyId = 30073;
        public const int UIA_WindowCanMinimizePropertyId = 30074;
        public const int UIA_WindowWindowInteractionStatePropertyId = 30075;
        public const int UIA_WindowWindowVisualStatePropertyId = 30076;
        public const int UIA_WindowIsModalPropertyId = 30077;
        public const int UIA_WindowIsTopmostPropertyId = 30078;

        public const int UIA_ToggleToggleStatePropertyId = 30086;

        public static bool IsWindowsPlatform()
        {
            return OperatingSystem.IsWindows();
        }
    }
}
