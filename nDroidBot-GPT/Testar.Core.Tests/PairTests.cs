using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class PairTests
    {
        [Test]
        public void FromCreatesExpectedPair()
        {
            int left = 10;
            int right = 20;
            Pair<int, int> result = Pair<int, int>.from(left, right);

            Assert.That(result.left(), Is.EqualTo(10));
            Assert.That(result.right(), Is.EqualTo(20));
        }

        [Test]
        public void EqualityWorks()
        {
            int left = 10;
            int right = 20;
            Pair<object, object> reference = new Pair<object, object>(left, right);
            Assert.That(reference.Equals(Pair<object, object>.from(left, right)), Is.True);
            Assert.That(reference.Equals(reference), Is.True);
        }
    }
}
