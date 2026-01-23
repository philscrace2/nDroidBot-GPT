using System.Collections.Generic;
using org.testar;
using org.testar.monkey.alayer.actions;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public abstract class Action : TaggableBase
    {
        public abstract void run(SUT system, State state, double duration);
        public abstract string toShortString();
        public abstract string toParametersString();
        public abstract string toString(params Role[] discardParameters);

        public void mapOriginWidget(Widget widget)
        {
            set(Tags.OriginWidget, widget);
        }

        public static string[] getActionRepresentation(Action action, string tab)
        {
            return getActionRepresentation(null, action, tab);
        }

        public static string[] getActionRepresentation(State? state, Action action, string tab)
        {
            string[] output = { string.Empty, string.Empty };
            Role? actionRole = action.get(Tags.Role, default(Role));
            if (actionRole != null)
            {
                output[0] += $"{tab}ROLE = {actionRole}\n";
                string roleCode = BriefActionRolesMap.map.TryGetValue(actionRole.ToString(), out var code)
                    ? code
                    : "??";
                output[1] = string.Format("{0,2} ", roleCode);
            }

            if (state != null)
            {
                var targets = action.get(Tags.Targets, default(List<Finder>));
                if (targets != null)
                {
                    foreach (Finder finder in targets)
                    {
                        Widget? widget = null;
                        try
                        {
                            widget = finder.apply(state);
                        }
                        catch
                        {
                            widget = null;
                        }

                        if (widget != null)
                        {
                            string targetRepresentation = widget.getRepresentation("\t\t");
                            output[0] += $"{tab}TARGET =\n{targetRepresentation}";
                            Role? widgetRole = widget.get(Tags.Role, default(Role));
                            string? title = widget.get(Tags.Title, default(string));
                            output[1] += string.Format("( {0," + CodingManager.ID_LENTGH + "}, {1,11}, {2} )",
                                widget.get(Tags.ConcreteID),
                                widgetRole?.ToString() ?? "???",
                                title == null ? "\"\"" : title);
                        }
                    }
                }
            }

            string? desc = action.get(Tags.Desc, default(string));
            if (desc != null)
            {
                output[0] += $"{tab}DESCRIPTION = {desc}\n";
            }

            string actionText = action.ToString().Replace("\r\n", "\n").Replace("\n", "\n\t\t");
            output[0] += $"{tab}TEXT = {actionText}\n";

            string paramsString = action.toParametersString()
                .Replace(")(", ",")
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("BUTTON1", string.Empty)
                .Replace("BUTTON3", string.Empty)
                .Replace(",,", string.Empty)
                .Replace(", ", ",");
            output[1] += " [ " + (paramsString == "," ? string.Empty : paramsString + " ]");
            return output;
        }
    }
}
