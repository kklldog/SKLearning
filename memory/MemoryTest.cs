using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using OllamaSharp;

namespace SKLearning.memory
{
    internal class MemoryTest
    {
        public async Task Run()
        {
            var modelName = "llama3.1:8b";
            var ollamaEndpoint = "http://localhost:11434";
            var ollamaApiClient = new OllamaApiClient(new Uri(ollamaEndpoint), modelName);
            var ollamaModelConfig = new OllamaModelConfig() { ModelName = modelName };
            var textEmbeddingGenerator = new OllamaTextEmbeddingGenerator(ollamaApiClient, ollamaModelConfig);

            var memory = new KernelMemoryBuilder()
                .WithOllamaTextGeneration(modelName, ollamaEndpoint)
                .WithOllamaTextEmbeddingGeneration(modelName, ollamaEndpoint)
#pragma warning disable KMEXP03
                .AddIngestionMemoryDb(new SimpleVectorDb(SimpleVectorDbConfig.Volatile, textEmbeddingGenerator))
#pragma warning restore KMEXP03
                .Build<MemoryServerless>();

            await memory.ImportWebPageAsync("https://microsoft.github.io/kernel-memory/", "KM");

            var query = Console.ReadLine();

            while (!string.IsNullOrEmpty(query))
            {
                var answer = await memory.AskAsync(query);

                Console.WriteLine(answer);

                query = Console.ReadLine();
            }
        }
    }
}
