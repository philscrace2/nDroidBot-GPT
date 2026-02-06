using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.environment;
using TestarEnvironment = org.testar.environment.Environment;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class EnvironmentTests
    {
        [Test]
        public void GetDefaultInstanceReturnsUnknown()
        {
            IEnvironment env = TestarEnvironment.getInstance();
            Assert.That(env, Is.Not.Null);
            Assert.That(env, Is.InstanceOf<UnknownEnvironment>());
            Assert.That(env.GetDisplayScale(0L), Is.EqualTo(1.0).Within(0.0001));
        }

        [Test]
        public void SetInstanceStoresValue()
        {
            IEnvironment testEnv = new TestEnvironment();
            TestarEnvironment.setInstance(testEnv);

            IEnvironment result = TestarEnvironment.getInstance();
            Assert.That(result, Is.SameAs(testEnv));
        }

        [Test]
        public void SetNullInstanceThrows()
        {
            Assert.That(() => TestarEnvironment.setInstance(null!), Throws.ArgumentException);
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
