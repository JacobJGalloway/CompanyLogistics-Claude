using ToDoAPI_Claude.Models.Interfaces;

namespace ToDoAPI_Claude.Models;

public class Item : IItem
{
    public string Identification { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
