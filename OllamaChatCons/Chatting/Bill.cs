using System;
using System.Collections.Generic;
using System.Text;

namespace OllamaChatCons.Chatting;

public class Bill
{
    public List<LineItem> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
}

public class LineItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
