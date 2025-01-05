using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;
using OllamaSharp;
using StackExchange.Redis;

namespace SKLearning.vector
{
    public class UserModel
    {
        [VectorStoreRecordKey]
        public string UserId { get; set; }

        [VectorStoreRecordData]
        public string UserName { get; set; }

        [VectorStoreRecordData]
        public string Hobby { get; set; }

        public string Description => $"{UserName}'s ID is {UserId} and hobby is {Hobby}";
        
        [VectorStoreRecordVector(1024, DistanceFunction.CosineDistance, IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    }

    internal class VectorTest
    {
        public async Task Run()
        {
            var modelName = "llama3.1:8b";
            var ollamaEndpoint = "http://localhost:11434";

            var vectorStore = new RedisVectorStore(
                ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(),
                new() { StorageType = RedisStorageType.HashSet });
            
            // init collection
            var collection = vectorStore.GetCollection<string, UserModel>("ks_user");
            await collection.CreateCollectionIfNotExistsAsync();
            
            // init embedding serivce
           var ollamaApiClient = new OllamaApiClient(new Uri(ollamaEndpoint), modelName);
           var embeddingGenerator = ollamaApiClient.AsTextEmbeddingGenerationService();

           // init user infos and vector
           var users = this.CreateUserModels();
            foreach (var user in users)
            {
                user.DescriptionEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(user.Description);
            }
            
            // insert or update
            foreach (var user in users)
            {
                await collection.UpsertAsync(user);           
            }

            // get
            var alice = await collection.GetAsync("1");
            Console.WriteLine(alice.UserName);

            var all = collection.GetBatchAsync(users.Select(x=>x.UserId));
            await foreach(var user in all)
            {
                Console.WriteLine(user.UserName);
            }
            
            // delete
            await collection.DeleteAsync("1");
            
            // search
            var vectorSearchOptions = new VectorSearchOptions
            {
                VectorPropertyName = nameof(UserModel.DescriptionEmbedding),
            };
            var query = await embeddingGenerator.GenerateEmbeddingAsync("Who hobby is swimming?");
            var searchResult = await collection.VectorizedSearchAsync(query,vectorSearchOptions);
            await foreach (var user in searchResult.Results)
            {
                Console.WriteLine(user.Record.UserName);
                Console.WriteLine(user.Score);
            }
        }

        private List<UserModel> CreateUserModels() => [
            new UserModel()
            {
                UserId = "1",
                UserName = "Alice",
                Hobby = "Reading",
            },
            new UserModel()
            {
                UserId = "2",
                UserName = "Bob",
                Hobby = "Swimming",
            },
            new UserModel()
            {
                UserId = "3",
                UserName = "Charlie",
                Hobby = "Running",
            },
            new UserModel()
            {
                UserId = "4",
                UserName = "David",
                Hobby = "Cycling",
            }
        ];
    }
}
