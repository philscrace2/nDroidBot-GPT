using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class AssertTests
    {
        [Test]
        public void HasTextNullThrows()
        {
            Assert.That(() => org.testar.monkey.Assert.hasText(null), Throws.ArgumentException);
        }

        [Test]
        public void HasTextEmptyThrows()
        {
            Assert.That(() => org.testar.monkey.Assert.hasText(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void HasTextValidDoesNotThrow()
        {
            Assert.That(() => org.testar.monkey.Assert.hasText("Good text"), Throws.Nothing);
        }
    }
}
