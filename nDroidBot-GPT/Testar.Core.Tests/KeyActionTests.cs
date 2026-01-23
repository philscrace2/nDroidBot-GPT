using NUnit.Framework;
using org.testar.monkey.alayer.actions;
using org.testar.monkey.alayer.devices;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class KeyActionTests
    {
        [Test]
        public void EqualsChecksTypeAndKey()
        {
            var action = new KeyDown(KBKeys.VK_CONTROL);
            Assert.That(action.Equals("Not a KeyAction"), Is.False);
            Assert.That(action.Equals(new KeyDown(KBKeys.VK_ALT)), Is.False);
            Assert.That(action.Equals(new KeyUp(KBKeys.VK_CONTROL)), Is.False);
            Assert.That(action.Equals(new KeyDown(KBKeys.VK_CONTROL)), Is.True);

            var upAction = new KeyUp(KBKeys.VK_CONTROL);
            Assert.That(upAction.Equals("Not a KeyAction"), Is.False);
            Assert.That(upAction.Equals(new KeyUp(KBKeys.VK_ALT)), Is.False);
            Assert.That(upAction.Equals(new KeyDown(KBKeys.VK_CONTROL)), Is.False);
            Assert.That(upAction.Equals(new KeyUp(KBKeys.VK_CONTROL)), Is.True);
        }
    }
}
