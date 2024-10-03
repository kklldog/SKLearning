using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

var endpoint = new Uri("http://localhost:11434");
var modelId = "llama3.1:8b";

var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070 
builder.Services.AddScoped<IChatCompletionService>(_ => new OllamaChatCompletionService(modelId, endpoint));

var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("This is a llama3 assistant ...");

while (true)
{
    Console.Write("You:");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    history.AddUserMessage(input);

    var contents = await chatService.GetChatMessageContentsAsync(history);

    foreach (var chatMessageContent in contents)
    {
        var content = chatMessageContent.Content;
        Console.WriteLine($"Ollama: {content}");
        history.AddMessage(chatMessageContent.Role, content ?? "");
    }
}