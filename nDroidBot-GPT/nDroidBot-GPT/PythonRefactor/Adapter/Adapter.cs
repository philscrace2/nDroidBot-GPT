using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDroidBot_GPT.PythonRefactor.Adapter
{
    public interface IAdapter
    {
        // Method to establish a connection
        void Connect();

        // Method to disconnect the connection
        void Disconnect();

        // Method to check connectivity status
        bool CheckConnectivity();

        // Method to set up the adapter
        void SetUp();

        // Method to tear down the adapter
        void TearDown();
    }

}
