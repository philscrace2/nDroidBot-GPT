using nDroidBot_GPT;
using NUnit.Framework;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models;

namespace nDroiBot_GPT_Tests
{
    [TestFixture]
    public class OpenAI_LLMIntegration_Tests
    {
        [Test]
        public void FetchAPIKeyFromEnvironmentVariable()
        {
            LLMIntegration lLMIntegration = new LLMIntegration();
            Assert.That(lLMIntegration.LoadApiKey(), Is.Not.Null);

        }

        [Test]
        public async Task SayHiToChatGPT()
        {
            LLMIntegration lLMIntegration = new LLMIntegration();
            lLMIntegration.InitializeAsync(new OpenAI.ChatGpt.Models.ChatGPTConfig() { MaxTokens = 300 });
            //Assert.That(lLMIntegration.SayHello("Hello chatgpt"), Is.Not.Null);
            Task<string> response = lLMIntegration.SayHello("Hello chatgpt");

            //Assert.IsNotNull(response);
            //Assert.IsInstanceOf<string>(response); // Ensuring the response is of string type
            //Assert.AreEqual("Expected response", response);

        }

        //[Test]
        //public async Task SayHello_ReturnsExpectedMessage()
        //{
        //    // Arrange
        //    string helloMessage = "Hello, AI!";

        //    // Act
        //    string response = await _chatService.SayHello(helloMessage);

        //    // Assert
        //    Assert.IsNotNull(response);
        //    Assert.IsInstanceOf<string>(response); // Ensuring the response is of string type
        //    Assert.AreEqual("Expected response", response); // Replace with your expected result
        //}

    }
}
