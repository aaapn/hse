using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Models;

namespace Vyshkat.Domain.Services;

/// <summary>Автоматический техосмотр</summary>
public sealed class CheckVehicle : ServiceCenter {
    public InspectionResult Inspect(Vehicle vehicle) {
        ArgumentNullException.ThrowIfNull(vehicle);

        if (!vehicle.Brakes) {
            return new InspectionResult(false, "Тормоза неисправны.");
        }

        if (!vehicle.Steering) {
            return new InspectionResult(false, "Рулевое управление неисправно.");
        }

        if (vehicle is ElectricVehicle electric && electric.BatteryCharge < 30) {
            return new InspectionResult(false, $"Заряд батареи {electric.BatteryCharge}%, требуется не менее 30%.");
        }

        return new InspectionResult(true, string.Empty);
    }
}
