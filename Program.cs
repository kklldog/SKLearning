#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0110

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKLearning;

var endpoint = new Uri("http://localhost:11434/v1/");
var modelId = "llama3.1:8b";

var httpClient = new HttpClient();
var builder = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId: modelId!, apiKey: null, endpoint: endpoint, httpClient: httpClient);
var kernel = builder.Build();
kernel.Plugins.AddFromType<WeatherPlugin>();
var settings = new OpenAIPromptExecutionSettings()
    { Temperature = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
var initMessage =
    "I am a weatherman. I can tell you the weather of any location. Try asking me about the weather of a location.";
history.AddSystemMessage(initMessage);
Console.WriteLine(initMessage);

while (true)
{
    Console.BackgroundColor = ConsoleColor.Black;
    Console.Write("You:");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    history.AddUserMessage(input);
    // Get the response from the AI
    var contents = await chatCompletionService.GetChatMessageContentsAsync(history, settings, kernel);

    foreach (var chatMessageContent in contents)
    {
        var content = chatMessageContent.Content;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"AI: {content}");
        history.AddMessage(chatMessageContent.Role, content ?? "");
    }
}