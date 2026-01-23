using System.IO;
using System.Text;
using NUnit.Framework;
using org.testar.monkey.alayer;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class TagTests
    {
        [Test]
        public void FromReturnsSameTag()
        {
            Tag<bool> tag1 = Tag<bool>.from<bool>("Boolean", typeof(bool));
            Tag<bool> tag2 = Tag<bool>.from<bool>("Boolean", typeof(bool));
            Assert.That(tag1, Is.EqualTo(tag2));
            Assert.That(tag2.type(), Is.EqualTo(typeof(bool)));
        }

        [Test]
        public void IsOneOfWorks()
        {
            var tag1 = Tag<string>.from<string>("firstString", typeof(string));
            var tag2 = Tag<string>.from<string>("secondString", typeof(string));
            var tag3 = Tag<string>.from<string>("thirdString", typeof(string));
            Assert.That(tag3.isOneOf(tag1, tag2), Is.False);
            Assert.That(tag2.isOneOf(tag1, tag2), Is.True);
        }

        [Test]
        public void WriteReadData()
        {
            string filePath = "dummy.dat";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var writer = new BinaryWriter(File.OpenWrite(filePath), Encoding.UTF8, leaveOpen: false))
            {
                writer.Write("firstString");
                writer.Write(typeof(string).AssemblyQualifiedName ?? string.Empty);
                writer.Write("secondString");
                writer.Write(typeof(string).AssemblyQualifiedName ?? string.Empty);
            }

            using (var reader = new BinaryReader(File.OpenRead(filePath), Encoding.UTF8, leaveOpen: false))
            {
                string name = reader.ReadString();
                string typeName = reader.ReadString();
                Type type = Type.GetType(typeName) ?? typeof(string);
                var tag = Tag<string>.from<string>(name, type);
                Assert.That(tag, Is.Not.Null);
            }

            File.Delete(filePath);
        }

        [Test]
        public void TagsNotEqualWhenDifferent()
        {
            Tag<string> tag1 = Tag<string>.from<string>("firstString", typeof(string));
            Tag<string> tag2 = Tag<string>.from<string>("secondString", typeof(string));
            Tag<bool> tag3 = Tag<bool>.from<bool>("firstString", typeof(bool));

            Assert.That(tag1.Equals(tag1), Is.True);
            Assert.That(tag1.Equals(null), Is.False);
            Assert.That(tag1.Equals(tag2), Is.False);
            Assert.That(tag1.Equals(tag3), Is.False);
        }
    }
}
