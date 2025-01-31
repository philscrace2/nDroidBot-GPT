using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nDroidBot_GPT
{
    using System.Collections.Generic;

    public class State
    {
        public string GUIInfo { get; set; }
        public string ProcessInfo { get; set; }
        public string Logs { get; set; }
        public List<Event> Transitions { get; set; } = new List<Event>();
    }

}
