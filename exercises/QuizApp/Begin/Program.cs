using QuizApp.Components;
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);



var innerChatClient = new AzureOpenAIClient(
        new Uri(builder.Configuration["AI:Endpoint"]!),
        new ApiKeyCredential(builder.Configuration["AI:Key"]!))
    .GetChatClient("gpt-4o-mini").AsIChatClient();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddChatClient(innerChatClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
