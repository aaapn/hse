using Vyshkat.Domain.Models;

namespace Vyshkat.Domain.Services;

/// <summary>Создаёт конкретный вид транспорта из проверенных данных</summary>
public sealed class VehicleFactory {
    public Vehicle CreateVehicle(VehicleType type, VehicleData data) {
        ArgumentNullException.ThrowIfNull(data);

        if (type == VehicleType.Bicycle) {
            if (data.BatteryCharge.HasValue || data.EnergyConsumption.HasValue) {
                throw new ArgumentException("Обычный велосипед не должен иметь батарею и расход электричества.", nameof(data));
            }

            return new Bicycle(data.Id, data.EaseOfUse, data.Brakes, data.Steering);
        }

        if (type is not (VehicleType.ElectricScooter or VehicleType.ElectricBike or VehicleType.ElectricCar)) {
            throw new ArgumentOutOfRangeException(nameof(type), "Неизвестный вид транспорта.");
        }

        if (!data.BatteryCharge.HasValue || !data.EnergyConsumption.HasValue) {
            throw new ArgumentException("Электротранспорту нужны заряд батареи и расход энергии.", nameof(data));
        }

        return type switch {
            VehicleType.ElectricScooter => new ElectricScooter(data.Id, data.EaseOfUse, data.Brakes,
                data.Steering, data.BatteryCharge.Value, data.EnergyConsumption.Value),
            VehicleType.ElectricBike => new ElectricBike(data.Id, data.EaseOfUse, data.Brakes,
                data.Steering, data.BatteryCharge.Value, data.EnergyConsumption.Value),
            VehicleType.ElectricCar => new ElectricCar(data.Id, data.EaseOfUse, data.Brakes,
                data.Steering, data.BatteryCharge.Value, data.EnergyConsumption.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
