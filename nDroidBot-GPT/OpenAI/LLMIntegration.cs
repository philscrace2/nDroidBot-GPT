using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace nDroidBot_GPT
{
    public class LLMIntegration
    {
        private OpenAIAPI _api;

        public LLMIntegration(string apiKey)
        {
            _api = new OpenAIAPI(apiKey);
        }

        public string GenerateActionFromState(string guiStateDescription)
        {
            var prompt = $"Given the following UI state, what should the next action be?\n{guiStateDescription}";
            var response = _api.Completions.CreateCompletionAsync(new CompletionRequest(prompt, 1)).Result;
            return response.Completions[0].Text.Trim();
        }
    }
}
