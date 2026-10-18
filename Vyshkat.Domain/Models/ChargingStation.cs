using Vyshkat.Domain.Interfaces;

namespace Vyshkat.Domain.Models;

public sealed class ChargingStation : InventoryItem, EnergyConsumer {
    public ChargingStation(int id, decimal energyConsumption) : base(id) {
        if (energyConsumption < 0) {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Расход энергии не может быть отрицательным.");
        }

        EnergyConsumption = energyConsumption;
    }

    public override string TypeName => "Зарядная станция";
    public decimal EnergyConsumption { get; }
}
