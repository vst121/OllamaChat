var builder = Host.CreateApplicationBuilder();

//// For Text
//builder.Services.AddChatClient(
//    new OllamaChatClient(
//        new Uri("http://localhost:11434"), 
//        "llama3.2"));


//// For Image
builder.Services.AddChatClient(
    new OllamaChatClient(
        new Uri("http://localhost:11434"),
        "llama3.2-vision:latest"));

//var app = builder.Build();

//var chatClient = app.Services.GetRequiredService<IChatClient>();

//var chatCompletion = await chatClient.CompleteAsync("What is .NET? Reply in 50 words max.");
//Console.WriteLine(chatCompletion.Message.Text);
//Console.ReadLine();

await OllamaTextChat.Run();

await OllamaImageChat.Run();




