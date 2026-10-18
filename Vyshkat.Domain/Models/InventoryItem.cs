using Vyshkat.Domain.Interfaces;

namespace Vyshkat.Domain.Models;

/// <summary>Базовый класс для имущества в инвентаре</summary>
public abstract class InventoryItem : IInventory{
    protected InventoryItem(int id) {
        if (id <= 0) {
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть положительным.");
        }
        
        Id = id;
    }

    public int Id { get; }
    public abstract string TypeName { get; }
}
