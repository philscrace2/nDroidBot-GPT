using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nDroidBot_GPT.PythonRefactor
{
    public class DroidBotScript
    {
        private static readonly string DEFAULT_ID = "default";
        private static readonly Regex IDENTIFIER_RE = new Regex(@"^[^\d\W]\w*\Z", RegexOptions.Unicode);

        private Dictionary<string, ViewSelector> views = new Dictionary<string, ViewSelector>();
        private Dictionary<string, StateSelector> states = new Dictionary<string, StateSelector>();
        private Dictionary<string, DroidBotOperation> operations = new Dictionary<string, DroidBotOperation>();
        private Dictionary<StateSelector, DroidBotAction> main = new Dictionary<StateSelector, DroidBotAction>();

        private Dictionary<string, object> scriptDict;

        public DroidBotScript(Dictionary<string, object> scriptDict)
        {
            this.scriptDict = scriptDict;
            Parse();
        }

        private void Parse()
        {
            ParseViews();
            ParseStates();
            ParseOperations();
            ParseMain();
            CheckDuplicatedIds();
        }

        private void ParseViews()
        {
            var viewsDict = scriptDict["views"] as Dictionary<string, object>;
            foreach (var viewId in viewsDict.Keys)
            {
                CheckIdentifierIsValid(viewId);
                var viewSelectorDict = viewsDict[viewId] as Dictionary<string, object>;
                views[viewId] = new ViewSelector(viewId, viewSelectorDict);
            }
        }

        private void ParseStates()
        {
            var statesDict = scriptDict["states"] as Dictionary<string, object>;
            foreach (var stateId in statesDict.Keys)
            {
                CheckIdentifierIsValid(stateId);
                var stateSelectorDict = statesDict[stateId] as Dictionary<string, object>;
                states[stateId] = new StateSelector(stateId, stateSelectorDict);
            }
        }

        private void ParseOperations()
        {
            var operationsDict = scriptDict["operations"] as Dictionary<string, object>;
            foreach (var operationId in operationsDict.Keys)
            {
                CheckIdentifierIsValid(operationId);
                var eventList = operationsDict[operationId] as List<Dictionary<string, object>>;
                operations[operationId] = new DroidBotOperation(operationId, eventList);
            }
        }

        private void ParseMain()
        {
            var mainDict = scriptDict["main"] as Dictionary<string, object>;
            foreach (var stateId in mainDict.Keys)
            {
                CheckIdentifierIsValid(stateId);
                var stateSelector = states[stateId];
                var action = mainDict[stateId] as List<object>;
                main[stateSelector] = action.Count > 1
                    ? new ProbabilisticDroidBotAction(action)
                    : new RoundRobinDroidBotAction(action);
            }
        }

        private void CheckDuplicatedIds()
        {
            var allIds = views.Keys.Concat(states.Keys).Concat(operations.Keys).ToList();
            var allIdsSet = new HashSet<string>(allIds);
            if (allIds.Count > allIdsSet.Count)
            {
                throw new ScriptSyntaxError("Duplicated identifier definition.");
            }
            if (allIdsSet.Contains(DEFAULT_ID))
            {
                throw new ScriptSyntaxError($"Defining reserved identifier: {DEFAULT_ID}");
            }
        }

        private void CheckIdentifierIsValid(string value)
        {
            if (!IDENTIFIER_RE.IsMatch(value))
            {
                throw new ScriptSyntaxError($"Invalid identifier: {value}");
            }
        }

        public DroidBotOperation GetOperationBasedOnState(DeviceState state)
        {
            foreach (var stateSelector in main.Keys)
            {
                if (stateSelector.Match(state))
                {
                    return main[stateSelector].GetNextOperation();
                }
            }
            return null;
        }
    }

    public class ScriptSyntaxError : Exception
    {
        public ScriptSyntaxError(string message) : base(message) { }
    }

    public class ViewSelector
    {
        public string Id { get; set; }
        private Regex textRe, resourceIdRe, classRe, contentDescRe;

        public ViewSelector(string id, Dictionary<string, object> selectorDict)
        {
            this.Id = id;
            Parse(selectorDict);
        }

        private void Parse(Dictionary<string, object> selectorDict)
        {
            foreach (var key in selectorDict.Keys)
            {
                switch (key)
                {
                    case "text":
                        textRe = new Regex(selectorDict[key].ToString());
                        break;
                    case "resource_id":
                        resourceIdRe = new Regex(selectorDict[key].ToString());
                        break;
                    case "class":
                        classRe = new Regex(selectorDict[key].ToString());
                        break;
                    case "content_desc":
                        contentDescRe = new Regex(selectorDict[key].ToString());
                        break;
                }
            }
        }

        public bool Match(Dictionary<string, object> viewDict)
        {
            // Matching logic for each view
            if (textRe != null && !textRe.IsMatch(viewDict["text"].ToString())) return false;
            if (resourceIdRe != null && !resourceIdRe.IsMatch(viewDict["resource_id"].ToString())) return false;
            if (classRe != null && !classRe.IsMatch(viewDict["class"].ToString())) return false;
            if (contentDescRe != null && !contentDescRe.IsMatch(viewDict["content_description"].ToString())) return false;
            return true;
        }
    }

    public abstract class DroidBotAction
    {
        public abstract DroidBotOperation GetNextOperation();
    }

    public class RoundRobinDroidBotAction : DroidBotAction
    {
        private List<DroidBotOperation> operations;
        private int currentIndex = 0;

        public RoundRobinDroidBotAction(List<object> action)
        {
            operations = new List<DroidBotOperation>();
            // Initialize operations based on action
        }

        public override DroidBotOperation GetNextOperation()
        {
            var operation = operations[currentIndex];
            currentIndex = (currentIndex + 1) % operations.Count;
            return operation;
        }
    }

    public class ProbabilisticDroidBotAction : DroidBotAction
    {
        private List<KeyValuePair<DroidBotOperation, double>> operations;

        public ProbabilisticDroidBotAction(List<object> action)
        {
            operations = new List<KeyValuePair<DroidBotOperation, double>>();
            // Initialize operations with probabilities
        }

        public override DroidBotOperation GetNextOperation()
        {
            var rand = new Random();
            double r = rand.NextDouble();
            foreach (var op in operations)
            {
                if (r < op.Value) return op.Key;
            }
            return null;
        }
    }



}
