using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey.alayer;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class VerdictTests
    {
        private sealed class DummyVisualizer : Visualizer
        {
            public void run(State state, Canvas canvas, Pen pen) { }
        }

        private sealed class FailVisualizer : Visualizer
        {
            public void run(State state, Canvas canvas, Pen pen) { }
        }

        [Test]
        public void ToStringIncludesSeverityAndInfo()
        {
            Verdict verdict = new Verdict(Verdict.Severity.OK, "This is a test verdict");
            Assert.That(verdict.ToString(), Is.EqualTo("severity: 0 info: This is a test verdict"));
        }

        [Test]
        public void JoinCombinesAsExpected()
        {
            var v1 = new Verdict(Verdict.Severity.OK, "Foo Bar");
            var v2 = new Verdict(Verdict.Severity.FAIL, "Bar", new FailVisualizer());
            var v3 = new Verdict(Verdict.Severity.OK, "Baz", new DummyVisualizer());

            Assert.That(v1, Is.Not.SameAs(v1.join(v2)));
            Assert.That(v3.join(v2).severity(), Is.EqualTo(1.0).Within(0.0001));
            Assert.That(v1.join(v2).info(), Is.EqualTo("Foo Bar"));
            Assert.That(v1.join(v3).info(), Is.EqualTo("Baz"));
            Assert.That(v2.join(v3).info(), Is.EqualTo("Bar\nBaz"));
            Assert.That(v2.join(v1).visualizer(), Is.InstanceOf<FailVisualizer>());
            Assert.That(v1.join(v2).visualizer(), Is.InstanceOf<FailVisualizer>());
        }

        [Test]
        public void EqualsChecksSeverityInfoAndVisualizer()
        {
            var v1 = Verdict.OK;
            var v2 = Verdict.FAIL;
            var v3 = new Verdict(Verdict.Severity.FAIL, "Failure");
            var v4 = new Verdict(Verdict.Severity.FAIL, "Different failure");

            Assert.That(Verdict.OK.Equals(Verdict.OK), Is.True);
            Assert.That(v1.Equals(Verdict.OK), Is.True);
            Assert.That(Verdict.OK.Equals(v1), Is.True);
            Assert.That(v2.Equals(Verdict.FAIL), Is.True);
            Assert.That(Verdict.FAIL.Equals(v2), Is.True);
            Assert.That(v1.Equals(Verdict.FAIL), Is.False);
            Assert.That(v3.Equals(Verdict.FAIL), Is.False);
            Assert.That(v3.Equals(v4), Is.False);
        }
    }
}
