// See https://aka.ms/new-console-template for more information
using nDroidBot_GPT;

Adapter adapter = new Adapter();
Brain brain = new Brain();
StateTransitionModel model = new StateTransitionModel();

// Example: dumping the views (UI hierarchy)
string viewDump = adapter.DumpViews();
Console.WriteLine(viewDump);

// Create some states and transitions (simplified)
State state1 = new State { GUIInfo = "Home Screen", ProcessInfo = "App Running", Logs = "Log Info" };
State state2 = new State { GUIInfo = "Settings Screen", ProcessInfo = "App Running", Logs = "Log Info" };

int state1Id = model.AddState(state1);
int state2Id = model.AddState(state2);

Event touchEvent = new Event { Input = "Touch Settings Button", Trigger = "State Change" };
model.AddTransition(state1Id, state2Id, touchEvent);

// Generate test inputs
brain.GenerateTestInputs();
