using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;



namespace nDroidBot_GPT
{
    public class LLMIntegration
    {
        ChatService _chatService;

        public string ApiKey { get; }

        public LLMIntegration()
        {
            ApiKey = LoadApiKey();
        }

        public LLMIntegration(string apiKey)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            Console.WriteLine("Welcome to ChatGPT Console!");

            var config = new ChatGPTConfig() { MaxTokens = 300 };
            ApiKey = apiKey;

            //Console.Write("User: ");
            //while (Console.ReadLine() is { } userMessage)
            //{
            //    var response = await chatService.GetNextMessageResponse(userMessage);
            //    Console.WriteLine($"ChatGPT: {response.Trim()}");
            //    Console.Write("User: ");
            //}
        }

        public async Task InitializeAsync(ChatGPTConfig config)
        {
            // This is where you initialize ChatService asynchronously
            _chatService = await ChatGPT.CreateInMemoryChat(ApiKey, config);
        }

        public async Task<string> GenerateActionFromState(string guiStateDescription)
        {
            var prompt = $"Given the following UI state, what should the next action be?\n{guiStateDescription}";

            var response = await _chatService.GetNextMessageResponse(prompt);

            // Extract the generated action (first completion result)
            return response;
        }

        public async Task<string> SayHello(string helloMessage)
        {
            return await _chatService.GetNextMessageResponse(helloMessage);            // Extract the generated action (first completion result)

        }

        public string LoadApiKey()
        {
            var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine);
            if (key is null)
            {
                Console.WriteLine("Please enter your OpenAI API key " +
                                    "(you can get it from https://platform.openai.com/account/api-keys): ");
                key = Console.ReadLine();
                if (key is null)
                {
                    throw new Exception("API key is not provided");
                }
            }

            return key;
        }
    }
}
