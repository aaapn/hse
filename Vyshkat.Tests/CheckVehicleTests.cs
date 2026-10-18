using Vyshkat.Domain.Models;
using Vyshkat.Domain.Services;
using Xunit;

namespace Vyshkat.Tests;

public sealed class CheckVehicleTests {
    private readonly CheckVehicle _service = new();

    [Fact]
    public void Inspect_ValidBicycle_IsApprovedWithoutBattery() {
        var bicycle = new Bicycle(1, 6, true, true);

        InspectionResult result = _service.Inspect(bicycle);

        Assert.True(result.Approved);
        Assert.Empty(result.Denied);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(29, false)]
    [InlineData(30, true)]
    [InlineData(100, true)]
    public void Inspect_BatteryAtBoundary_UsesThirtyPercentRule(int charge, bool approved) {
        var scooter = new ElectricScooter(2, 5, true, true, charge, 2m);

        InspectionResult result = _service.Inspect(scooter);

        Assert.Equal(approved, result.Approved);
        if (!approved)
            Assert.Contains("30%", result.Denied);
        else
            Assert.Empty(result.Denied);
    }

    [Fact]
    public void Inspect_BrokenBrakes_RejectsVehicleWithReason() {
        var car = new ElectricCar(3, 10, false, true, 90, 12m);

        InspectionResult result = _service.Inspect(car);

        Assert.False(result.Approved);
        Assert.Contains("Тормоза", result.Denied);
    }

    [Fact]
    public void Inspect_BrokenSteering_RejectsVehicleWithReason() {
        var bike = new ElectricBike(4, 9, true, false, 90, 5m);

        InspectionResult result = _service.Inspect(bike);

        Assert.False(result.Approved);
        Assert.Contains("Рулевое", result.Denied);
    }

    [Fact]
    public void Inspect_BrokenBrakesAndSteering_ExplainsFirstFailure() {
        var bicycle = new Bicycle(5, 8, false, false);

        InspectionResult result = _service.Inspect(bicycle);

        Assert.False(result.Approved);
        Assert.Contains("Тормоза", result.Denied);
    }

    [Fact]
    public void Inspect_NullVehicle_ThrowsArgumentNullException() {
        Assert.Throws<ArgumentNullException>(() => _service.Inspect(null!));
    }
}
