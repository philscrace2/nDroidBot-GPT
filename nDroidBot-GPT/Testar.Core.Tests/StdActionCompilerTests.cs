using System.Collections.Generic;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class StdActionCompilerTests
    {
        [Test]
        public void HitShortcutKeyPressesAndReleasesInOrder()
        {
            var compiler = new StdActionCompiler();
            var keys = new List<KBKeys> { KBKeys.VK_CONTROL, KBKeys.VK_SHIFT, KBKeys.VK_F };
            var action = compiler.hitShortcutKey(keys);

            string expected = "(VK_CONTROL)(VK_SHIFT)(VK_F)(VK_F)(VK_SHIFT)(VK_CONTROL)";
            Assert.That(action.toParametersString(), Is.EqualTo(expected));
        }
    }
}
