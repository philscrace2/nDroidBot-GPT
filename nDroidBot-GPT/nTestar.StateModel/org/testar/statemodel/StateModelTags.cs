using org.testar.monkey.alayer;

namespace org.testar.statemodel
{
    public sealed class StateModelTags : TaggableBase
    {
        private StateModelTags()
        {
        }

        public static readonly Tag<bool> StateModelEnabled = Tag<bool>.from<bool>("StateModelEnabled", typeof(bool),
            "Enable or disable the State Model feature");

        public static readonly Tag<string> DataStore = Tag<string>.from<string>("DataStore", typeof(string),
            "The graph database we use to store the State Model: OrientDB");

        public static readonly Tag<string> DataStoreType = Tag<string>.from<string>("DataStoreType", typeof(string),
            "The mode we use to connect to the database: remote or plocal");

        public static readonly Tag<string> DataStoreServer = Tag<string>.from<string>("DataStoreServer", typeof(string),
            "IP address to connect if we desired to use remote mode");

        public static readonly Tag<string> DataStoreDirectory = Tag<string>.from<string>("DataStoreDirectory", typeof(string),
            "Path of the directory on which local OrientDB exists, if we use plocal mode");

        public static readonly Tag<string> DataStoreDb = Tag<string>.from<string>("DataStoreDB", typeof(string),
            "The name of the desired database on which we want to store the State Model.");

        public static readonly Tag<string> DataStoreUser = Tag<string>.from<string>("DataStoreUser", typeof(string),
            "User credential to authenticate TESTAR in OrientDB");

        public static readonly Tag<string> DataStorePassword = Tag<string>.from<string>("DataStorePassword", typeof(string),
            "Password credential to authenticate TESTAR in OrientDB");

        public static readonly Tag<string> DataStoreMode = Tag<string>.from<string>("DataStoreMode", typeof(string),
            "Indicate how TESTAR should store the model objects in the database: instant, delayed, hybrid, none");

        public static readonly Tag<string> ActionSelectionAlgorithm = Tag<string>.from<string>("ActionSelectionAlgorithm", typeof(string),
            "State Model Action Selection mechanism to explore the SUT: random or unvisited");

        public static readonly Tag<bool> StateModelStoreWidgets = Tag<bool>.from<bool>("StateModelStoreWidgets", typeof(bool),
            "Save all widget tree information in the State Model every time TESTAR discovers a new Concrete State");

        public static readonly Tag<bool> ResetDataStore = Tag<bool>.from<bool>("ResetDataStore", typeof(bool),
            "WARNING: Delete all existing State Models from the selected database before creating a new one");
    }
}
