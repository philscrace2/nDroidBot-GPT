using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar.monkey;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class DragTests
    {
        [Test]
        public void DragConstructorStoresValues()
        {
            Drag drag = new Drag(0.0, 0.0, 5.0, 5.0);
            Assert.That(drag.getFromX(), Is.EqualTo(0.0).Within(0.0001));
            Assert.That(drag.getFromY(), Is.EqualTo(0.0).Within(0.0001));
            Assert.That(drag.getToX(), Is.EqualTo(5.0).Within(0.0001));
            Assert.That(drag.getToY(), Is.EqualTo(5.0).Within(0.0001));
        }

        [Test]
        public void EqualsMatchesSameValues()
        {
            Drag d1 = new Drag(1.0, 1.0, 2.0, 2.0);
            Drag d2 = new Drag(1.0, 1.0, 2.0, 2.0);
            Assert.That(d1, Is.EqualTo(d2));
        }

        [Test]
        public void NotEqualsDetectsDifferentValues()
        {
            Drag d1 = new Drag(1.0, 1.0, 2.0, 2.0);
            Drag d2 = new Drag(1.0, 1.0, 3.0, 3.0);
            Assert.That(d1, Is.Not.EqualTo(d2));
        }
    }
}
