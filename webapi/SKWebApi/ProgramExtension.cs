using Microsoft.SemanticKernel;

namespace SKWebApi
{
    public static class ProgramExtension
    {
        public static IServiceCollection AddKernelBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IKernelBuilder>(sp =>
            {
                var deployment = "gpt3.5";
                var endpoint = "http://localhost:11434/v1/";
                var apikey = "123";
                var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(deployment, endpoint, apikey);

                return builder;
            });

            return services;
        }

        public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
        {
            services.AddSingleton<IKernelBuilder>(sp =>
            {
                var deployment = "gpt3.5";
                var endpoint = "http://localhost:11434/v1/";
                var apikey = "123";
                var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(deployment, endpoint, apikey);

                return builder;
            });

            services.AddScoped<Kernel>(sp =>
            {
                var builder = sp.GetRequiredService<IKernelBuilder>();
                return builder.Build();
            });

            return services;
        }

        public static IServiceCollection AddSemanticKernel2(this IServiceCollection services)
        {
            services.AddSingleton<KernelProvider>();

            services.AddScoped<Kernel>(sp =>
            {
                var provider = sp.GetRequiredService<KernelProvider>();

                return provider.GetKernel();
            });

            return services;
        }

        public static IServiceCollection AddSemanticKernel3(this IServiceCollection services)
        {
            var deployment = "gpt3.5";
            var endpoint = "http://localhost:11434/v1/";
            var apikey = "123";
            services.AddKernel();
            services.AddOpenAIChatCompletion(deployment, endpoint, apikey);

            return services;
        }

        class KernelProvider
        {
            private readonly Kernel _kernel;

            public KernelProvider()
            {
                var deployment = "gpt3.5";
                var endpoint = "http://localhost:11434/v1/";
                var apikey = "123";
                var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(deployment, endpoint, apikey);

                _kernel = builder.Build();
            }

            public Kernel GetKernel()
            {
                return _kernel.Clone();
            }
        }
    }
}
