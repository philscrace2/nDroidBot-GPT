using NUnit.Framework;
using org.testar.environment;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class EnvironmentTests
    {
        [Test]
        public void GetDefaultInstanceReturnsUnknown()
        {
            IEnvironment env = Environment.getInstance();
            Assert.That(env, Is.Not.Null);
            Assert.That(env, Is.InstanceOf<UnknownEnvironment>());
            Assert.That(env.GetDisplayScale(0L), Is.EqualTo(1.0).Within(0.0001));
        }

        [Test]
        public void SetInstanceStoresValue()
        {
            IEnvironment testEnv = new TestEnvironment();
            Environment.setInstance(testEnv);

            IEnvironment result = Environment.getInstance();
            Assert.That(result, Is.SameAs(testEnv));
        }

        [Test]
        public void SetNullInstanceThrows()
        {
            Assert.That(() => Environment.setInstance(null!), Throws.ArgumentException);
        }

        private sealed class TestEnvironment : IEnvironment
        {
            public double GetDisplayScale(long windowHandle)
            {
                return 0;
            }
        }
    }
}
