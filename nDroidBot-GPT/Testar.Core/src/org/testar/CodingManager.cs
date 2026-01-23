using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.testar.monkey.alayer;
using Action = org.testar.monkey.alayer.Action;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.exceptions;

namespace org.testar
{
    public static class CodingManager
    {
        public const int ID_LENTGH = 24;
        public const string CONCRETE_ID = "ConcreteID";
        public const string ABSTRACT_ID = "AbstractID";

        public const string ABSTRACT_R_ID = "Abs(R)ID";
        public const string ABSTRACT_R_T_ID = "Abs(R,T)ID";
        public const string ABSTRACT_R_T_P_ID = "Abs(R,T,P)ID";

        public const string ID_PREFIX_CONCRETE = "C";
        public const string ID_PREFIX_ABSTRACT = "A";
        public const string ID_PREFIX_ABSTRACT_R = "R";
        public const string ID_PREFIX_ABSTRACT_R_T = "T";
        public const string ID_PREFIX_ABSTRACT_R_T_P = "P";

        public const string ID_PREFIX_STATE = "S";
        public const string ID_PREFIX_WIDGET = "W";
        public const string ID_PREFIX_ACTION = "A";

        private static readonly ITag[] TAGS_ABSTRACT_R_ID = { Tags.Role };
        private static readonly ITag[] TAGS_ABSTRACT_R_T_ID = { Tags.Role, Tags.Title };
        private static readonly ITag[] TAGS_ABSTRACT_R_T_P_ID = { Tags.Role, Tags.Title, Tags.Path };

        public static readonly Role[] ROLES_ABSTRACT_ACTION =
        {
            ActionRoles.Type,
            ActionRoles.KeyDown,
            ActionRoles.KeyUp
        };

        private static ITag[] customTagsForConcreteId = Array.Empty<ITag>();
        private static ITag[] customTagsForAbstractId = Array.Empty<ITag>();
        private static readonly ITag[] defaultAbstractStateTags = { StateManagementTags.WidgetControlType };

        public static void setCustomTagsForConcreteId(ITag[] tags)
        {
            customTagsForConcreteId = tags.OrderBy(tag => tag.name()).ToArray();
        }

        public static void setCustomTagsForAbstractId(ITag[] tags)
        {
            customTagsForAbstractId = tags.OrderBy(tag => tag.name()).ToArray();
        }

        public static ITag[] getCustomTagsForAbstractId()
        {
            return customTagsForAbstractId;
        }

        public static ITag[] getCustomTagsForConcreteId()
        {
            return customTagsForConcreteId;
        }

        public static ITag[] getDefaultAbstractStateTags()
        {
            return defaultAbstractStateTags;
        }

        public static void buildIDs(Widget widget)
        {
            if (widget.parent() != null)
            {
                widget.set(Tags.ConcreteID, ID_PREFIX_WIDGET + ID_PREFIX_CONCRETE + codify(widget, customTagsForConcreteId));
                widget.set(Tags.AbstractID, ID_PREFIX_WIDGET + ID_PREFIX_ABSTRACT + codify(widget, customTagsForAbstractId));
                widget.set(Tags.Abstract_R_ID, ID_PREFIX_WIDGET + ID_PREFIX_ABSTRACT_R + codify(widget, TAGS_ABSTRACT_R_ID));
                widget.set(Tags.Abstract_R_T_ID, ID_PREFIX_WIDGET + ID_PREFIX_ABSTRACT_R_T + codify(widget, TAGS_ABSTRACT_R_T_ID));
                widget.set(Tags.Abstract_R_T_P_ID, ID_PREFIX_WIDGET + ID_PREFIX_ABSTRACT_R_T_P + codify(widget, TAGS_ABSTRACT_R_T_P_ID));
                return;
            }

            if (widget is State state)
            {
                var concreteId = new StringBuilder();
                var abstractId = new StringBuilder();
                var abstractRoleId = new StringBuilder();
                var abstractRoleTitleId = new StringBuilder();
                var abstractRoleTitlePathId = new StringBuilder();

                foreach (Widget child in state)
                {
                    if (ReferenceEquals(child, widget))
                    {
                        continue;
                    }

                    buildIDs(child);
                    concreteId.Append(child.get(Tags.ConcreteID));
                    abstractId.Append(child.get(Tags.AbstractID));
                    abstractRoleId.Append(child.get(Tags.Abstract_R_ID));
                    abstractRoleTitleId.Append(child.get(Tags.Abstract_R_T_ID));
                    abstractRoleTitlePathId.Append(child.get(Tags.Abstract_R_T_P_ID));
                }

                widget.set(Tags.ConcreteID, ID_PREFIX_STATE + ID_PREFIX_CONCRETE + lowCollisionID(concreteId.ToString()));
                widget.set(Tags.AbstractID, ID_PREFIX_STATE + ID_PREFIX_ABSTRACT + lowCollisionID(abstractId.ToString()));
                widget.set(Tags.Abstract_R_ID, ID_PREFIX_STATE + ID_PREFIX_ABSTRACT_R + lowCollisionID(abstractRoleId.ToString()));
                widget.set(Tags.Abstract_R_T_ID, ID_PREFIX_STATE + ID_PREFIX_ABSTRACT_R_T + lowCollisionID(abstractRoleTitleId.ToString()));
                widget.set(Tags.Abstract_R_T_P_ID, ID_PREFIX_STATE + ID_PREFIX_ABSTRACT_R_T_P + lowCollisionID(abstractRoleTitlePathId.ToString()));
            }
        }

        public static void buildIDs(State state, ISet<Action> actions)
        {
            foreach (Action action in actions)
            {
                action.set(Tags.ConcreteID, ID_PREFIX_ACTION + ID_PREFIX_CONCRETE + codify(state.get(Tags.ConcreteID), action));
            }

            var roleCounter = new Dictionary<Role, int>();
            foreach (Action action in actions.OrderBy(action =>
                     action.get(Tags.OriginWidget).get(Tags.Path, string.Empty)))
            {
                updateRoleCounter(action, roleCounter);
                action.set(Tags.AbstractID, ID_PREFIX_ACTION + ID_PREFIX_ABSTRACT +
                                             lowCollisionID(state.get(Tags.AbstractID) + getAbstractActionIdentifier(action, roleCounter)));
            }
        }

        public static void buildEnvironmentActionIDs(State state, Action action)
        {
            action.set(Tags.ConcreteID, ID_PREFIX_ACTION + ID_PREFIX_CONCRETE + codify(state.get(Tags.ConcreteID), action));
            action.set(Tags.AbstractID, ID_PREFIX_ACTION + ID_PREFIX_ABSTRACT + codify(state.get(Tags.AbstractID), action, ROLES_ABSTRACT_ACTION));
        }

        private static void updateRoleCounter(Action action, Dictionary<Role, int> roleCounter)
        {
            Role role;
            try
            {
                role = action.get(Tags.OriginWidget).get(Tags.Role);
            }
            catch (NoSuchTagException)
            {
                role = action.get(Tags.Role, Roles.Invalid);
            }

            roleCounter[role] = roleCounter.TryGetValue(role, out int count) ? count + 1 : 1;
        }

        private static string getAbstractActionIdentifier(Action action, Dictionary<Role, int> roleCounter)
        {
            Role role;
            try
            {
                role = action.get(Tags.OriginWidget).get(Tags.Role);
            }
            catch (NoSuchTagException)
            {
                role = action.get(Tags.Role, Roles.Invalid);
            }

            return role.ToString() + roleCounter.GetValueOrDefault(role, 999);
        }

        private static string codify(Widget widget, params ITag[] tags)
        {
            return lowCollisionID(getTaggedString(widget, tags));
        }

        private static string getTaggedString(Widget widget, params ITag[] tags)
        {
            var sb = new StringBuilder();
            foreach (ITag tag in tags)
            {
                sb.Append(GetTagValue(widget, tag));
                if (StateManagementTags.isStateManagementTag(tag) &&
                    StateManagementTags.getTagGroup(tag) == StateManagementTags.Group.ControlPattern)
                {
                    foreach (ITag child in StateManagementTags.getChildTags(tag).OrderBy(t => t.name()))
                    {
                        sb.Append(GetTagValue(widget, child));
                    }
                }
            }

            return sb.ToString();
        }

        private static string codify(string stateId, Action action, params Role[] discardParameters)
        {
            return lowCollisionID(stateId + action.toString(discardParameters));
        }

        private static object? GetTagValue(Widget widget, ITag tag)
        {
            var method = widget.GetType().GetMethods()
                .FirstOrDefault(m => m.Name == "get" && m.IsGenericMethod && m.GetParameters().Length == 2);
            if (method == null)
            {
                return null;
            }

            var constructed = method.MakeGenericMethod(tag.type());
            return constructed.Invoke(widget, new object?[] { tag, null });
        }

        private static string lowCollisionID(string text)
        {
            uint crc = Crc32.Compute(Encoding.UTF8.GetBytes(text));
            string hash = ToUnsignedString(JavaStringHashCode(text), 36);
            string lengthHex = text.Length.ToString("x");
            return hash + lengthHex + crc;
        }

        public static string getAbstractStateModelHash(string applicationName, string applicationVersion)
        {
            var abstractTags = getCustomTagsForAbstractId().OrderBy(tag => tag.name()).ToArray();
            var sb = new StringBuilder();
            foreach (var tag in abstractTags)
            {
                sb.Append(tag.name());
            }

            sb.Append(applicationName);
            sb.Append(applicationVersion);
            return lowCollisionID(sb.ToString());
        }

        private static int JavaStringHashCode(string text)
        {
            unchecked
            {
                int hash = 0;
                foreach (char c in text)
                {
                    hash = 31 * hash + c;
                }

                return hash;
            }
        }

        private static string ToUnsignedString(int value, int radix)
        {
            const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";
            uint u = unchecked((uint)value);
            if (u == 0)
            {
                return "0";
            }

            var sb = new StringBuilder();
            while (u > 0)
            {
                uint rem = u % (uint)radix;
                sb.Insert(0, digits[(int)rem]);
                u /= (uint)radix;
            }

            return sb.ToString();
        }

        private static class Crc32
        {
            private static readonly uint[] Table = CreateTable();

            public static uint Compute(byte[] data)
            {
                uint crc = 0xFFFFFFFFu;
                foreach (byte b in data)
                {
                    uint idx = (crc ^ b) & 0xFFu;
                    crc = (crc >> 8) ^ Table[idx];
                }

                return crc ^ 0xFFFFFFFFu;
            }

            private static uint[] CreateTable()
            {
                const uint poly = 0xEDB88320u;
                var table = new uint[256];
                for (uint i = 0; i < table.Length; i++)
                {
                    uint crc = i;
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (crc >> 1) ^ poly : crc >> 1;
                    }

                    table[i] = crc;
                }

                return table;
            }
        }
    }
}
