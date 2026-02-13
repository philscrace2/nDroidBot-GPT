using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using org.testar;
using org.testar.monkey;
using org.testar.monkey.alayer;
using org.testar.monkey.alayer.actions;
using org.testar.plugin;
using org.testar.settings;
using Action = org.testar.monkey.alayer.Action;

namespace org.testar.protocols
{
    public class GenericUtilsProtocol : ClickFilterLayerProtocol
    {
        protected static double SCROLL_ARROW_SIZE = 36;
        protected static double SCROLL_THICK = 16;

        private Regex? clickFilterPattern;
        private readonly Dictionary<string, bool> clickFilterMatches = new();

        protected GenericUtilsProtocol()
        {
        }

        protected override void initialize(Settings settings)
        {
            base.initialize(settings);
        }

        protected override void preSequencePreparations()
        {
        }

        protected override SUT startSystem()
        {
            return base.startSystem();
        }

        protected override void beginSequence(SUT system, State state)
        {
            base.beginSequence(system, state);
        }

        protected override State getState(SUT system)
        {
            return base.getState(system);
        }

        protected override Verdict getVerdict(State state)
        {
            return base.getVerdict(state);
        }

        protected override ISet<Action> deriveActions(SUT system, State state)
        {
            return base.deriveActions(system, state);
        }

        protected override Action selectAction(State state, ISet<Action> actions)
        {
            return base.selectAction(state, actions);
        }

        protected override bool executeAction(SUT system, State state, Action action)
        {
            return base.executeAction(system, state, action);
        }

        protected override bool moreActions(State state)
        {
            return base.moreActions(state);
        }

        protected override bool moreSequences()
        {
            return base.moreSequences();
        }

        protected override void finishSequence()
        {
            base.finishSequence();
        }

        protected override void stopSystem(SUT system)
        {
            base.stopSystem(system);
        }

        protected override void postSequenceProcessing()
        {
            base.postSequenceProcessing();
        }

        protected virtual void buildStateIdentifiers(State state)
        {
        }

        protected virtual void buildStateActionsIdentifiers(State state, ISet<Action> actions)
        {
        }

        protected virtual void buildEnvironmentActionIdentifiers(State state, Action action)
        {
        }

        protected virtual bool waitAndLeftClickWidgetWithMatchingTag(string tagName, string value, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            if (state.childCount() == 0)
            {
                return false;
            }

            if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase))
            {
                if (!tagName.StartsWith("Web", StringComparison.Ordinal))
                {
                    tagName = "Web" + tagName;
                }
            }

            foreach (var tag in state.child(0).tags())
            {
                if (string.Equals(tag.name(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return waitAndLeftClickWidgetWithMatchingTag(tag, value, state, system, maxNumberOfRetries, waitBetween);
                }
            }

            Console.WriteLine($"Matching widget was not found, {tagName}={value}");
            return false;
        }

        protected virtual bool waitAndLeftClickWidgetWithMatchingTags(Dictionary<string, string> tagValues, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTags(tagValues, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredClickAction(state, widget);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            return false;
        }

        protected virtual bool waitAndLeftClickWidgetWithMatchingTag(ITag tag, string value, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTag(tag, value, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredClickAction(state, widget);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            Console.WriteLine($"Matching widget was not found, {tag}={value}");
            printTagValuesOfWidgets(tag, state);
            return false;
        }

        protected virtual bool waitLeftClickAndTypeIntoWidgetWithMatchingTag(string tagName, string value, string textToType, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            if (state.childCount() == 0)
            {
                return false;
            }

            if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase))
            {
                if (!tagName.StartsWith("Web", StringComparison.Ordinal))
                {
                    tagName = "Web" + tagName;
                }
            }

            foreach (var tag in state.child(0).tags())
            {
                if (string.Equals(tag.name(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return waitLeftClickAndTypeIntoWidgetWithMatchingTag(tag, value, textToType, state, system, maxNumberOfRetries, waitBetween);
                }
            }

            Console.WriteLine($"Matching widget was not found, {tagName}={value}");
            return false;
        }

        protected virtual bool waitLeftClickAndTypeIntoWidgetWithMatchingTags(Dictionary<string, string> tagValues, string textToType, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTags(tagValues, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredTypeAction(state, widget, textToType, true);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            return false;
        }

        protected virtual bool waitLeftClickAndTypeIntoWidgetWithMatchingTag(ITag tag, string value, string textToType, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTag(tag, value, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredTypeAction(state, widget, textToType, true);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            Console.WriteLine($"Matching widget was not found, {tag}={value}");
            printTagValuesOfWidgets(tag, state);
            return false;
        }

        protected virtual bool waitLeftClickAndPasteIntoWidgetWithMatchingTag(string tagName, string value, string textToPaste, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            if (state.childCount() == 0)
            {
                return false;
            }

            if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase))
            {
                if (!tagName.StartsWith("Web", StringComparison.Ordinal))
                {
                    tagName = "Web" + tagName;
                }
            }

            foreach (var tag in state.child(0).tags())
            {
                if (string.Equals(tag.name(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return waitLeftClickAndPasteIntoWidgetWithMatchingTag(tag, value, textToPaste, state, system, maxNumberOfRetries, waitBetween);
                }
            }

            Console.WriteLine($"Matching widget was not found, {tagName}={value}");
            return false;
        }

        protected virtual bool waitLeftClickAndPasteIntoWidgetWithMatchingTags(Dictionary<string, string> tagValues, string textToPaste, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTags(tagValues, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredPasteAction(state, widget, textToPaste, true);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            return false;
        }

        protected virtual bool waitLeftClickAndPasteIntoWidgetWithMatchingTag(ITag tag, string value, string textToPaste, State state, SUT system, int maxNumberOfRetries, double waitBetween)
        {
            int numberOfRetries = 0;
            while (numberOfRetries < maxNumberOfRetries)
            {
                var widget = getWidgetWithMatchingTag(tag, value, state);
                if (widget != null)
                {
                    var triggeredAction = triggeredPasteAction(state, widget, textToPaste, true);
                    buildStateActionsIdentifiers(state, new HashSet<Action> { triggeredAction });
                    executeAction(system, state, triggeredAction);
                    return true;
                }

                Util.pause(waitBetween);
                state = getState(system);
                numberOfRetries++;
            }

            Console.WriteLine($"Matching widget was not found, {tag}={value}");
            printTagValuesOfWidgets(tag, state);
            return false;
        }

        protected virtual Widget? getWidgetWithMatchingTag(string tagName, string value, State state)
        {
            if (state.childCount() == 0)
            {
                return null;
            }

            if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase))
            {
                if (!tagName.StartsWith("Web", StringComparison.Ordinal))
                {
                    tagName = "Web" + tagName;
                }
            }

            foreach (var tag in state.child(0).tags())
            {
                if (string.Equals(tag.name(), tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return getWidgetWithMatchingTag(tag, value, state);
                }
            }

            return null;
        }

        protected virtual Widget? getWidgetWithMatchingTags(Dictionary<string, string> tagValues, State state)
        {
            if (state.childCount() == 0)
            {
                return null;
            }

            var tagLookup = new Dictionary<string, ITag>(StringComparer.OrdinalIgnoreCase);
            foreach (var tagNameValue in tagValues.Keys)
            {
                var tagName = tagNameValue;
                if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase) &&
                    !tagName.StartsWith("Web", StringComparison.Ordinal))
                {
                    tagName = "Web" + tagName;
                }

                bool tagFound = false;
                foreach (var tag in state.child(0).tags())
                {
                    if (string.Equals(tag.name(), tagName, StringComparison.OrdinalIgnoreCase))
                    {
                        tagLookup[tagName] = tag;
                        tagFound = true;
                        break;
                    }
                }

                if (!tagFound)
                {
                    Console.WriteLine($"Error: could not find tag for tag name {tagName}");
                    return null;
                }
            }

            foreach (var widget in state)
            {
                var tagsFound = new List<string>();
                var webTagValues = new Dictionary<string, string>(tagValues, StringComparer.OrdinalIgnoreCase);

                foreach (var tagNameValue in tagValues.Keys)
                {
                    var tagName = tagNameValue;
                    if (NativeLinker.getPLATFORM_OS().Contains(OperatingSystems.WEBDRIVER, StringComparison.OrdinalIgnoreCase) &&
                        !tagName.StartsWith("Web", StringComparison.Ordinal))
                    {
                        var value = tagValues[tagName];
                        webTagValues.Remove(tagName);
                        tagName = "Web" + tagName;
                        webTagValues[tagName] = value;
                    }

                    if (!tagLookup.TryGetValue(tagName, out var tag))
                    {
                        continue;
                    }

                    var valueToMatch = webTagValues[tagName];
                    var tagValue = GetTagValue(widget, tag);
                    if (tagValue != null && string.Equals(tagValue.ToString(), valueToMatch, StringComparison.Ordinal))
                    {
                        tagsFound.Add(tagName);
                    }
                }

                if (tagsFound.Count == webTagValues.Count)
                {
                    return widget;
                }
            }

            return null;
        }

        protected virtual Widget? getWidgetWithMatchingTag(ITag tag, string value, State state)
        {
            foreach (var widget in state)
            {
                var tagValue = GetTagValue(widget, tag);
                if (tagValue == null)
                {
                    continue;
                }

                var tagString = tagValue.ToString() ?? string.Empty;
                if (string.Equals(tagString, value, StringComparison.Ordinal))
                {
                    return widget;
                }

                if (tagString.Contains(value, StringComparison.Ordinal))
                {
                    return widget;
                }
            }

            return null;
        }

        protected virtual void printTagValuesOfWidgets(ITag tag, State state)
        {
            foreach (var widget in state)
            {
                var tagValue = GetTagValue(widget, tag);
                if (tagValue == null)
                {
                    continue;
                }

                var description = widget.get(Tags.Desc, string.Empty);
                Console.WriteLine($"{tag}={tagValue}; Description of the widget={description}");
            }
        }

        protected virtual void addSlidingActions(ISet<Action> actions, StdActionCompiler ac, Widget widget)
        {
            var drags = widget.scrollDrags(SCROLL_ARROW_SIZE, SCROLL_THICK);
            if (drags == null)
            {
                return;
            }

            foreach (var drag in drags)
            {
                actions.Add(ac.slideFromTo(
                    new AbsolutePosition(Point.from(drag.getFromX(), drag.getFromY())),
                    new AbsolutePosition(Point.from(drag.getToX(), drag.getToY())),
                    widget));
            }
        }

        protected virtual DerivedActions addSlidingActions(DerivedActions derived, StdActionCompiler ac, Drag[] drags, Widget widget)
        {
            foreach (var drag in drags)
            {
                derived.addAvailableAction(ac.slideFromTo(
                    new AbsolutePosition(Point.from(drag.getFromX(), drag.getFromY())),
                    new AbsolutePosition(Point.from(drag.getToX(), drag.getToY())),
                    widget));
            }

            return derived;
        }

        protected virtual bool isClickable(Widget widget)
        {
            var role = widget.get(Tags.Role, Roles.Widget);
            return Role.isOneOf(role, NativeLinker.getNativeClickableRoles());
        }

        protected virtual bool isTypeable(Widget widget)
        {
            return NativeLinker.isNativeTypeable(widget);
        }

        protected virtual bool isUnfiltered(Widget widget)
        {
            if (!Util.hitTest(widget, 0.5, 0.5))
            {
                return false;
            }

            var tagsToFilter = settings().get(ConfigTags.TagsToFilter, new List<string>());
            foreach (var tagToFilter in tagsToFilter)
            {
                var tagValue = string.Empty;
                foreach (var tag in widget.tags())
                {
                    var value = GetTagValue(widget, tag);
                    if (value != null && string.Equals(tag.name(), tagToFilter, StringComparison.Ordinal))
                    {
                        tagValue = value.ToString() ?? string.Empty;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(tagValue))
                {
                    continue;
                }

                if (clickFilterPattern == null)
                {
                    var pattern = settings().get(ConfigTags.ClickFilter, string.Empty);
                    clickFilterPattern = new Regex($"^(?:{pattern})$", RegexOptions.CultureInvariant);
                }

                if (!clickFilterMatches.TryGetValue(tagValue, out var matches))
                {
                    matches = clickFilterPattern.IsMatch(tagValue);
                    clickFilterMatches[tagValue] = matches;
                }

                if (matches)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual List<Widget> getTopWidgets(State state)
        {
            var topWidgets = new List<Widget>();
            var maxZIndex = state.get(Tags.MaxZIndex, 0.0);
            foreach (var widget in state)
            {
                if (Math.Abs(widget.get(Tags.ZIndex, 0.0) - maxZIndex) < double.Epsilon)
                {
                    topWidgets.Add(widget);
                }
            }

            return topWidgets;
        }

        protected virtual bool isNOP(Action action)
        {
            var asString = action.ToString();
            return asString != null && asString.Equals(NOP.NOP_ID, StringComparison.Ordinal);
        }

        protected virtual bool isESC(Action action)
        {
            var role = action.get(Tags.Role, default(Role));
            if (role != null && role.isA(ActionRoles.HitKey))
            {
                var desc = action.get(Tags.Desc, default(string));
                if (!string.IsNullOrEmpty(desc) && desc.Contains("VK_ESCAPE", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        protected override ISet<Action> preSelectAction(SUT system, State state, ISet<Action> actions)
        {
            if (actions.Count == 0)
            {
                actions = retryDeriveAction(system, 5, 1);
            }

            return base.preSelectAction(system, state, actions);
        }

        protected virtual ISet<Action> retryDeriveAction(SUT system, int maxRetries, int waitingSeconds)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                Console.WriteLine("Wait and retry to derive GUI actions...");
                Util.pause(waitingSeconds);
                var newState = getState(system);
                var newActions = deriveActions(system, newState);
                if (newActions.Count > 0)
                {
                    buildStateActionsIdentifiers(newState, newActions);
                    return newActions;
                }
            }

            return new HashSet<Action>();
        }

        protected virtual Action triggeredClickAction(State state, Widget widget)
        {
            StdActionCompiler ac = new AnnotatingActionCompiler();
            return ac.leftClickAt(widget);
        }

        protected virtual Action triggeredTypeAction(State state, Widget widget, string textToType, bool replaceText)
        {
            StdActionCompiler ac = new AnnotatingActionCompiler();
            return ac.clickTypeInto(widget, textToType, replaceText);
        }

        protected virtual Action triggeredPasteAction(State state, Widget widget, string textToPaste, bool replaceText)
        {
            StdActionCompiler ac = new AnnotatingActionCompiler();
            return ac.pasteTextInto(widget, textToPaste, replaceText);
        }

        protected Settings settings()
        {
            return settingsRef();
        }

        private static object? GetTagValue(Widget widget, ITag tag)
        {
            var method = widget.GetType().GetMethods()
                .FirstOrDefault(m => m.Name == "get" && m.IsGenericMethod && m.GetParameters().Length == 2);
            if (method == null)
            {
                return null;
            }

            var constructed = method.MakeGenericMethod(tag.type());
            return constructed.Invoke(widget, new object?[] { tag, null });
        }
    }
}
