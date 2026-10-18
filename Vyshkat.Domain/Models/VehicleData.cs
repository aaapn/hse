namespace Vyshkat.Domain.Models;

/// <summary>Параметры создания транспорта, передаваемые в фабрику</summary>
public sealed class VehicleData {
    public VehicleData(int id, int easeOfUse, bool brakes, bool steering, int? batteryCharge = null, decimal? energyConsumption = null) {
        if (id <= 0){
            throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть положительным.");
        }
        if (easeOfUse is < 1 or > 10) {
            throw new ArgumentOutOfRangeException(nameof(easeOfUse), "Простота должна быть от 1 до 10.");
        }
        if (batteryCharge is < 0 or > 100) {
            throw new ArgumentOutOfRangeException(nameof(batteryCharge), "Заряд должен быть от 0 до 100%.");
        }
        if (energyConsumption < 0) {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Расход энергии не может быть отрицательным.");
        }

        Id = id;
        EaseOfUse = easeOfUse;
        Brakes = brakes;
        Steering = steering;
        BatteryCharge = batteryCharge;
        EnergyConsumption = energyConsumption;
    }

    public int Id { get; }
    public int EaseOfUse { get; }
    public bool Brakes { get; }
    public bool Steering { get; }
    public int? BatteryCharge { get; }
    public decimal? EnergyConsumption { get; }
}
