namespace OllamaChatCons.Chatting;

public static class OllamaImageChat
{
    public static async Task Run()
    {
        var chatClient = new OllamaChatClient("http://localhost:11434");
        var chatHistory = new List<ChatMessage>();

        var message = new ChatMessage(
            ChatRole.User, "What's in this image?");

        message.Contents.Add(
            new DataContent(
                File.ReadAllBytes("D://TempTest//Bill_Images/Bill_1.png"),
                "image/png"));

        var response = await chatClient.GetResponseAsync([message]);

        Console.WriteLine(response.Text);




    }
}

