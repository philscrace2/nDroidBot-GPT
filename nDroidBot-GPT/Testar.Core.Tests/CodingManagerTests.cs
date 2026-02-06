using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using org.testar;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.stub;

namespace Testar.Core.Tests
{
    [TestFixture]
    public class CodingManagerTests
    {
        private static readonly StateStub state = new();
        private static readonly WidgetStub widget = new();

        [OneTimeSetUp]
        public void InitializeCodingIds()
        {
            ITag[] abstractTags = { Tags.Role, Tags.Path };
            CodingManager.setCustomTagsForAbstractId(abstractTags);

            ITag[] concreteTags = { Tags.Role, Tags.Title, Tags.Path };
            CodingManager.setCustomTagsForConcreteId(concreteTags);

            state.addChild(widget);
            widget.setParent(state);

            widget.set(Tags.Role, Roles.Button);
            widget.set(Tags.Title, "Submit");
            widget.set(Tags.Path, "0,0,1");
        }

        [Test]
        public void InitialCodingIds()
        {
            Assert.That(CodingManager.getDefaultAbstractStateTags()[0].ToString(), Is.EqualTo("Widget control type"));
            Assert.That(CodingManager.getCustomTagsForAbstractId().Length, Is.EqualTo(2));
            Assert.That(string.Join(", ", Array.ConvertAll(CodingManager.getCustomTagsForAbstractId(), t => t.ToString())),
                Is.EqualTo("Path, Role"));

            Assert.That(CodingManager.getCustomTagsForConcreteId().Length, Is.EqualTo(3));
            Assert.That(string.Join(", ", Array.ConvertAll(CodingManager.getCustomTagsForConcreteId(), t => t.ToString())),
                Is.EqualTo("Path, Role, Title"));
        }

        [Test]
        public void WidgetCodingIds()
        {
            CodingManager.buildIDs(widget);
            Assert.That(widget.get(Tags.AbstractID), Is.EqualTo("WAane37vb337119275"));
            Assert.That(widget.get(Tags.ConcreteID), Is.EqualTo("WCxrhgw3113942939805"));
        }

        [Test]
        public void StateCodingIds()
        {
            CodingManager.buildIDs(state);
            Assert.That(state.get(Tags.AbstractID), Is.EqualTo("SA1fl7scw122940428572"));
            Assert.That(state.get(Tags.ConcreteID), Is.EqualTo("SCr5r0gz142938361104"));
        }

        [Test]
        public void ActionCodingIds()
        {
            org.testar.monkey.alayer.Action action = new PasteText("paste");
            action.set(Tags.OriginWidget, widget);
            var actions = new HashSet<Action> { action };

            CodingManager.buildIDs(state, actions);
            Assert.That(action.get(Tags.AbstractID), Is.EqualTo("AA1sahtjg1c4157641605"));
            Assert.That(action.get(Tags.ConcreteID), Is.EqualTo("ACd7vwql27266850918"));
        }
    }
}
