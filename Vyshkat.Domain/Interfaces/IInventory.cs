namespace Vyshkat.Domain.Interfaces;

/// <summary>Общее имущество, стоящее на балансе</summary>
public interface IInventory
{
    int Id { get; }
    string TypeName { get; }
}
