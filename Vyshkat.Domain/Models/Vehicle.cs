namespace Vyshkat.Domain.Models;

/// <summary>Общие свойства любого прокатного транспорта</summary>
public abstract class Vehicle : InventoryItem {
    protected Vehicle(int id, int easeOfUse, bool brakes, bool steering) : base(id) {
        if (easeOfUse is < 1 or > 10) {
            throw new ArgumentOutOfRangeException(nameof(easeOfUse), "Простота должна быть от 1 до 10.");
        }

        EaseOfUse = easeOfUse;
        Brakes = brakes;
        Steering = steering;
    }

    public int EaseOfUse { get; }
    public bool Brakes { get; }
    public bool Steering { get; }
    public bool SuitableForBeginners => EaseOfUse >= 6;
}

/// <summary>Обычный велосипед</summary>
public sealed class Bicycle : Vehicle {
    public Bicycle(int id, int easeOfUse, bool brakes, bool steering) : base(id, easeOfUse, brakes, steering) { }
    public override string TypeName => "Велосипед";
}
