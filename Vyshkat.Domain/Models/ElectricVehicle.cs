using Vyshkat.Domain.Interfaces;

namespace Vyshkat.Domain.Models;

/// <summary>Общие характеристики электрического транспорта</summary>
public abstract class ElectricVehicle : Vehicle, EnergyConsumer {
    protected ElectricVehicle(int id, int easeOfUse, bool brakes, bool steering, int batteryCharge, decimal energyConsumption) : base(id, easeOfUse, brakes, steering) {
        if (batteryCharge is < 0 or > 100){
            throw new ArgumentOutOfRangeException(nameof(batteryCharge), "Заряд должен быть от 0 до 100.");
        }
        if (energyConsumption < 0) {
            throw new ArgumentOutOfRangeException(nameof(energyConsumption), "Расход энергии не может быть отрицательным.");
        }

        BatteryCharge = batteryCharge;
        EnergyConsumption = energyConsumption;
    }

    public int BatteryCharge { get; }
    public decimal EnergyConsumption { get; }
}

public sealed class ElectricScooter : ElectricVehicle {
    public ElectricScooter(int id, int easeOfUse, bool brakes, bool steering, int batteryCharge, decimal energyConsumption) : base(id, easeOfUse, brakes, steering, batteryCharge, energyConsumption) { }
    public override string TypeName => "Электросамокат";
}

public sealed class ElectricBike : ElectricVehicle {
    public ElectricBike(int id, int easeOfUse, bool brakes, bool steering, int batteryCharge, decimal energyConsumption) : base(id, easeOfUse, brakes, steering, batteryCharge, energyConsumption) { }
    public override string TypeName => "Электровелосипед";
}

public sealed class ElectricCar : ElectricVehicle {
    public ElectricCar(int id, int easeOfUse, bool brakes, bool steering, int batteryCharge, decimal energyConsumption) : base(id, easeOfUse, brakes, steering, batteryCharge, energyConsumption) { }
    public override string TypeName => "Электрокар";
}
