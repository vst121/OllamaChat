namespace OllamaChatCons.Chatting;

public static class OllamaImageChat
{
    public static async Task Run()
    {
        var chatClient = new OllamaChatClient("http://localhost:11434");

        var imagePath = "D://TempTest//Bill_Images/Bill_1.png";

        var message1 = new ChatMessage(
            ChatRole.User, "What's in this image?");

        message1.Contents.Add(
            new DataContent(
                File.ReadAllBytes(imagePath),
                "image/png"));

        var response1 = await chatClient.GetResponseAsync([message1]);

        Console.WriteLine(response1.Text);

        // Next Step: Asking for JSON Output

        var message2 = new ChatMessage(ChatRole.User,
            """
            Extract all line items from this receipt.
            Respond in JSON format with this structure:
            {
                "items": [
                            {
                        "name": "item name",
                        "quantity": 1.500,
                        "unitPrice": 0.00,
                        "totalPrice": 0.00
                    }
                ],
                "subtotal": 0.00
            }
            """);

        message2.Contents.Add(
            new DataContent(
                File.ReadAllBytes(imagePath),
                "image/png"));

        var response2 = await chatClient.GetResponseAsync([message2]);

        Console.WriteLine(response2.Text);


        // Next Step: Iterating on the System Prompt

        var systemMessage = new ChatMessage(ChatRole.System,
            """
            You are a bill parsing assistant. Extract all line items from the bill image.
            For each line item, extract the name, quantity, unit price, and total price.
            Quantity can be a decimal number (e.g. weight in kg like 0.550 or 1.105).
            Extract the subtotal which is the final total amount shown on the bill.
            IMPORTANT: Read every digit exactly as printed on the bill.
            Pay very close attention to each decimal digit - do NOT round or approximate.
            For example, if the bill shows 1.105, report exactly 1.105, not 1.1 or 1.2.
            Verify that quantity * unitPrice = totalPrice for each line item.
            Don't invent items that aren't on the bill.

            DECIMAL FORMAT: bills may use different number formats depending on locale.
            - Some use period as decimal separator: 7,499.00
            - Some use comma as decimal separator: 7.499,00
            First, detect which format the bill uses by examining the numbers on it.
            Then, always output numbers in the JSON using a period as the decimal separator.
            For example: 7499.00, not 7.499,00 or 7,499.00.
            """);

        var response3 = await chatClient.GetResponseAsync<Bill>(
            [systemMessage, message2],
            new ChatOptions { Temperature = 0 });

        if (response3.Result is { } bill)
        {
            Console.WriteLine(
                $"\nExtracted {bill.Items.Count} line items:");

            foreach (var item in bill.Items)
            {
                Console.WriteLine(
                    $"  {item.Name} - " +
                    $"Qty: {item.Quantity} x {item.UnitPrice:C}" +
                    $" = {item.TotalPrice:C}");
            }

            Console.WriteLine($"  Subtotal: {bill.Subtotal:C}");
        }

    }
}

