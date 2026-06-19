using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTestar.Desktop.Winforms.mvp
{
    public sealed class MainScreenModel
    {
        public string SutConnector { get; set; }
        public string SutConnectorType { get; set; }
        public int NumberOfSequences { get; set; }
        public int SequenceActions { get; set; }
        public bool VisualizeActionsOnGui { get; set; }
        public bool AlwaysCompileProtocol { get; set; }
        public string Protocol { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string OverrideDisplayScale { get; set; }

        public IReadOnlyList<string> Protocols { get; set; }

        public static MainScreenModel CreateDefault()
        {
            return new MainScreenModel
            {
                SutConnector = "\"https://para.testar.org/\"",
                SutConnectorType = "COMMAND_LINE",
                NumberOfSequences = 1,
                SequenceActions = 10,
                VisualizeActionsOnGui = false,
                AlwaysCompileProtocol = true,
                Protocol = "webdriver_llm_state_widgets_evaluator",
                ApplicationName = "parabank",
                ApplicationVersion = "login",
                OverrideDisplayScale = string.Empty,
                Protocols = new[]
                {
                    "webdriver_llm_state_widgets_evaluator",
                    "webdriver_llm_state_model_evaluator",
                    "desktop_generic_protocol"
                }
            };
        }
    }
}
