using SKWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddKernelBuilder();
//builder.Services.AddSemanticKernel();
//builder.Services.AddSemanticKernel2();
builder.Services.AddSemanticKernel3();

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
