using System.Runtime.InteropServices;
using org.testar.monkey.alayer;

namespace org.testar.plugin
{
    public static class NativeLinker
    {
        public static string getPLATFORM_OS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OperatingSystems.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OperatingSystems.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OperatingSystems.Mac;
            }

            return OperatingSystems.Unknown;
        }

        public static Role[] getNativeClickableRoles()
        {
            return new[]
            {
                Roles.Button,
                Roles.StateButton,
                Roles.ToggleButton,
                Roles.Item,
                Roles.Slider
            };
        }

        public static bool isNativeTypeable(Widget widget)
        {
            var role = widget.get(Tags.Role, Roles.Widget);
            return Role.isOneOf(role, Roles.Text);
        }
    }
}
