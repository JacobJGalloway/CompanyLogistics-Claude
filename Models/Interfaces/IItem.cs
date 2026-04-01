namespace ToDoAPI_Claude.Models.Interfaces;

public interface IItem : IModel
{
    string Identification { get; set; }
    string Category { get; set; }
}
