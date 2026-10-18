using Vyshkat.Domain.Models;
using Vyshkat.Domain.Services;
using Xunit;

namespace Vyshkat.Tests;

public sealed class VehicleFactoryTests {
    private readonly VehicleFactory _factory = new();

    [Fact]
    public void CreateVehicle_Bicycle_ReturnsBicycleWithoutElectricData() {
        var data = new VehicleData(1, 6, true, true);

        Vehicle vehicle = _factory.CreateVehicle(VehicleType.Bicycle, data);

        Assert.IsType<Bicycle>(vehicle);
        Assert.Equal(1, vehicle.Id);
        Assert.Equal("Велосипед", vehicle.TypeName);
    }

    [Theory]
    [InlineData(VehicleType.ElectricScooter, typeof(ElectricScooter))]
    [InlineData(VehicleType.ElectricBike, typeof(ElectricBike))]
    [InlineData(VehicleType.ElectricCar, typeof(ElectricCar))]
    public void CreateVehicle_ElectricKinds_ReturnCorrectConcreteTypes(VehicleType type, Type expectedType) {
        var data = new VehicleData(2, 8, true, true, 70, 4.5m);

        Vehicle vehicle = _factory.CreateVehicle(type, data);

        Assert.IsType(expectedType, vehicle);
        Assert.Equal(70, ((ElectricVehicle)vehicle).BatteryCharge);
        Assert.Equal(4.5m, ((ElectricVehicle)vehicle).EnergyConsumption);
    }

    [Fact]
    public void CreateVehicle_BicycleWithElectricData_Throws() {
        var data = new VehicleData(1, 5, true, true, 70, 2m);
        Assert.Throws<ArgumentException>(() => _factory.CreateVehicle(VehicleType.Bicycle, data));
    }

    [Theory]
    [InlineData(null, 2)]
    [InlineData(30, null)]
    public void CreateVehicle_ElectricWithoutRequiredData_Throws(int? charge, int? energy) {
        var data = new VehicleData(1, 7, true, true, charge, energy);
        Assert.Throws<ArgumentException>(() => _factory.CreateVehicle(VehicleType.ElectricScooter, data));
    }

    [Fact]
    public void CreateVehicle_UnknownKind_Throws() {
        var data = new VehicleData(1, 6, true, true);
        Assert.Throws<ArgumentOutOfRangeException>(() => _factory.CreateVehicle((VehicleType)100, data));
    }

    [Fact]
    public void CreateVehicle_NullData_Throws() {
        Assert.Throws<ArgumentNullException>(() => _factory.CreateVehicle(VehicleType.Bicycle, null!));
    }

    [Fact]
    public void VehicleData_InvalidId_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VehicleData(0, 5, true, true));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void VehicleData_InvalidEase_Throws(int ease) {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VehicleData(1, ease, true, true));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void VehicleData_InvalidBattery_Throws(int charge) {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VehicleData(1, 5, true, true, charge, 0m));
    }

    [Fact]
    public void VehicleData_NegativeEnergy_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VehicleData(1, 5, true, true, 80, -1m));
    }

    [Fact]
    public void ChargingStation_NegativeEnergy_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingStation(1, -0.1m));
    }

    [Fact]
    public void InventoryItem_NonPositiveId_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Helmet(-1));
    }

    [Fact]
    public void ElectricVehicle_NegativeConsumption_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ElectricCar(1, 5, true, true, 50, -1m));
    }

    [Fact]
    public void InspectionResult_RequiresDeniedReasonOnFailure() {
        Assert.Throws<ArgumentException>(() => new InspectionResult(false, string.Empty));
    }
}
