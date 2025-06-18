﻿using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ClientModel;

// Set up DI etc
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Configuration.AddUserSecrets<Program>();
hostBuilder.Services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Register an IChatClient

// For GitHub Models or Azure OpenAI:
var aiConfig = hostBuilder.Configuration.GetRequiredSection("AI");
var innerChatClient = new AzureOpenAIClient(new Uri(aiConfig["Endpoint"]!), new ApiKeyCredential(aiConfig["Key"]!))
    .GetChatClient("gpt-4o-mini").AsIChatClient();

// Or for OpenAI Platform:
// var aiConfig = hostBuilder.Configuration.GetRequiredSection("AI");
// var innerChatClient = new OpenAI.Chat.ChatClient("gpt-4o-mini", aiConfig["Key"]!).AsIChatClient();

// Or for Ollama:
// IChatClient innerChatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "llava");

hostBuilder.Services.AddChatClient(innerChatClient)
    .UseFunctionInvocation();

// Run the app
var app = hostBuilder.Build();
var chatClient = app.Services.GetRequiredService<IChatClient>();
var trafficImages = Directory.GetFiles("../../../traffic-cam", "*.jpg");

var raiseAlert = AIFunctionFactory.Create((string cameraName, string alertReason) =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("*** CAMERA ALERT ***");
    Console.WriteLine($"Camera {cameraName}: {alertReason}");
    Console.ForegroundColor = ConsoleColor.White;
}, "RaiseAlert");
var chatOptions = new ChatOptions { Tools = [raiseAlert] };

// Multi-modality (images)
foreach (var imagePath in trafficImages)
{
    var name = Path.GetFileNameWithoutExtension(imagePath);

    var message = new ChatMessage(ChatRole.User, $$"""
        Extract information from this image from camera {{name}}.
        Raise an alert only if the camera is broken or if there's something highly unusual or dangerous,
        not just because of traffic volume.
        """);
    message.Contents.Add(new DataContent(File.ReadAllBytes(imagePath), "image/jpg"));
    var response = await chatClient.GetResponseAsync<TrafficCamResult>([message], chatOptions);

    if (response.TryGetResult(out var result))
    {
        Console.WriteLine($"{name} status: {result.Status} (cars: {result.NumCars}, trucks: {result.NumTrucks})");
    }
}

class TrafficCamResult
{
    public TrafficStatus Status { get; set; }
    public int NumCars { get; set; }
    public int NumTrucks { get; set; }

    public enum TrafficStatus { Clear, Flowing, Congested, Blocked };
}
