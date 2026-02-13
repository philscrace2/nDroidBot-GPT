using System.Collections.Generic;
using org.testar;
using org.testar.monkey;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;

namespace org.testar.monkey.alayer
{
    public sealed class Tags : TagsBase
    {
        private Tags() { }

        public static readonly Tag<Role> Role = from<Role>("Role", typeof(Role));
        public static readonly Tag<string> Title = from<string>("Title", typeof(string));
        public static readonly Tag<string> Path = from<string>("Path", typeof(string));
        public static readonly Tag<string> ConcreteID = from<string>(CodingManager.CONCRETE_ID, typeof(string));
        public static readonly Tag<string> AbstractID = from<string>(CodingManager.ABSTRACT_ID, typeof(string));
        public static readonly Tag<string> Abstract_R_ID = from<string>(CodingManager.ABSTRACT_R_ID, typeof(string));
        public static readonly Tag<string> Abstract_R_T_ID = from<string>(CodingManager.ABSTRACT_R_T_ID, typeof(string));
        public static readonly Tag<string> Abstract_R_T_P_ID = from<string>(CodingManager.ABSTRACT_R_T_P_ID, typeof(string));

        public static readonly Tag<string> Desc = from<string>("Desc", typeof(string));
        public static readonly Tag<Widget> OriginWidget = from<Widget>("OriginWidget", typeof(Widget));
        public static readonly Tag<List<Finder>> Targets = from<List<Finder>>("Targets", typeof(List<Finder>));
        public static readonly Tag<string> InputText = from<string>("InputText", typeof(string));
        public static readonly Tag<Visualizer> Visualizer = from<Visualizer>("Visualizer", typeof(Visualizer));
        public static readonly Tag<HitTester> HitTester = from<HitTester>("HitTester", typeof(HitTester));
        public static readonly Tag<Shape> Shape = from<Shape>("Shape", typeof(Shape));
        public static readonly Tag<Keyboard> StandardKeyboard = from<Keyboard>("StandardKeyboard", typeof(Keyboard));
        public static readonly Tag<org.testar.monkey.alayer.devices.Mouse> StandardMouse =
            from<org.testar.monkey.alayer.devices.Mouse>("StandardMouse", typeof(org.testar.monkey.alayer.devices.Mouse));
        public static readonly Tag<bool> HasStandardMouse = from<bool>("HasStandardMouse", typeof(bool));
        public static readonly Tag<string> ToolTipText = from<string>("ToolTipText", typeof(string));
        public static readonly Tag<string> ScreenshotPath = from<string>("ScreenshotPath", typeof(string));
        public static readonly Tag<Position[]> Slider = from<Position[]>("Slider", typeof(Position[]));
        public static readonly Tag<double> ZIndex = from<double>("ZIndex", typeof(double));
        public static readonly Tag<double> MaxZIndex = from<double>("MaxZIndex", typeof(double));
        public static readonly Tag<double> MinZIndex = from<double>("MinZIndex", typeof(double));
        public static readonly Tag<long> HWND = from<long>("HWND", typeof(long));
        public static readonly Tag<long> PID = from<long>("PID", typeof(long));
        public static readonly Tag<long> TimeStamp = from<long>("TimeStamp", typeof(long));
        public static readonly Tag<long> StateRenderTime = from<long>("StateRenderTime", typeof(long));
        public static readonly Tag<Verdict> OracleVerdict = from<Verdict>("OracleVerdict", typeof(Verdict));
        public static readonly Tag<bool> IsRunning = from<bool>("IsRunning", typeof(bool));
        public static readonly Tag<bool> NotResponding = from<bool>("NotResponding", typeof(bool));
        public static readonly Tag<bool> Foreground = from<bool>("Foreground", typeof(bool));
        public static readonly Tag<object> SystemActivator = from<object>("SystemActivator", typeof(object));
        public static readonly Tag<List<Pair<long, string>>> RunningProcesses =
            from<List<Pair<long, string>>>("RunningProcesses", typeof(List<Pair<long, string>>));
        public static readonly Tag<bool> Enabled = from<bool>("Enabled", typeof(bool));
        public static readonly Tag<bool> Blocked = from<bool>("Blocked", typeof(bool));
        public static readonly Tag<string> TargetID = from<string>("TargetID", typeof(string));
    }
}
