using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), "llama3.2"));

var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();

//var chatCompletion = await chatClient.CompleteAsync("What is .NET? Reply in 50 words max.");
//Console.WriteLine(chatCompletion.Message.Text);
//Console.ReadLine();

var chatHistory = new List<ChatMessage>();

while (true)
{
    Console.WriteLine("Enter your prompt:");
    var userPrompt = Console.ReadLine();

    if (userPrompt.ToLower() == "quit")
    {
        break;
    }

    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    Console.WriteLine("Response from AI:");
    var chatResponse = "";
    await foreach (var item in chatClient.CompleteStreamingAsync(chatHistory))
    {
        // We're streaming the response, so we get each message as it arrives
        Console.Write(item.Text);
        chatResponse += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
    Console.WriteLine();
}

Directory.CreateDirectory("posts");
var filePath = "C:\\DotnetProjects8\\OllamaChat\\OllamaChatCons\\Posts\\TodayPosts.txt";

using (StreamWriter writer = new StreamWriter(filePath))
{
    foreach (var message in chatHistory)
    {
        writer.WriteLine(message.ToString());
    }
}

var myPosts = "";
using (StreamReader reader = new StreamReader(filePath))
{
    while(!reader.EndOfStream)
    {
        myPosts += reader.ReadLine();
    }
}

var myPosts2 = File.ReadAllText(filePath);

string prompt = $$"""
         You will receive an input text and the desired output format.
         You need to analyze the text and produce the desired output format.
         You not allow to change code, text, or other references.

         # Desired response

         Only provide a RFC8259 compliant JSON response following this format without deviation.

         {
            "title": "Title pulled from the front matter section",
            "summary": "Summarize the article in no more than 100 words"
         }

         # Article content:

         {{myPosts2}}
         """;

var chatCompletion = await chatClient.CompleteAsync(prompt);
Console.WriteLine(chatCompletion.Message.Text);
Console.WriteLine(Environment.NewLine);



Console.ReadLine();

var prompt2 = $$"""
          You will receive an input text and the desired output format.
          You need to analyze the text and produce the desired output format.
          You not allow to change code, text, or other references.

          # Desired response

          Only provide a RFC8259 compliant JSON response following this format without deviation.

          {
             "title": "Title pulled from the front matter section",
             "tags": "Array of tags based on analyzing the article content. Tags should be lowercase."
          }

          # Article content:

          {{myPosts2}}
          """;

var chatCompletion2 = await chatClient.CompleteAsync<PostCategory>(prompt2);

Console.WriteLine(
      $"{chatCompletion2.Result.Title}. Tags: {string.Join(",", chatCompletion2.Result.Tags)}");

Console.ReadLine(); 


class PostCategory
{
    public string Title { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
}





