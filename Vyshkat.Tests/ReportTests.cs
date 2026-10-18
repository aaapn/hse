using Vyshkat.Domain.Models;
using Vyshkat.Domain.Services;
using Xunit;

namespace Vyshkat.Tests;

public sealed class ReportTests {
    [Fact]
    public void GetInventoryReport_CountsVehiclesAndThingsSeparately() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        manager.AddVehicle(new Bicycle(1, 6, true, true));
        manager.AddVehicle(new ElectricCar(2, 4, true, true, 50, 12m));
        manager.AddItem(new Helmet(3));
        manager.AddItem(new ChargingStation(4, 2m));
        manager.AddItem(new FirstAidKit(5));

        InventoryReport report = manager.GetInventoryReport();

        Assert.Equal(2, report.VehicleCount);
        Assert.Equal(3, report.ItemCount);
        Assert.Equal(5, report.TotalCount);
    }

    [Fact]
    public void GetInventoryReport_EmptyPark_AllCountsZero() {
        var manager = new InventoryManager(new FakeServiceCenter(true));

        InventoryReport report = manager.GetInventoryReport();

        Assert.Equal(0, report.VehicleCount);
        Assert.Equal(0, report.ItemCount);
        Assert.Equal(0, report.TotalCount);
    }

    [Fact]
    public void GetEnergyConsumption_SumsAllEnergyConsumersIncludingChargingStation() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        manager.AddVehicle(new ElectricScooter(1, 7, true, true, 30, 1.5m));
        manager.AddVehicle(new ElectricBike(2, 8, true, true, 40, 2.25m));
        manager.AddVehicle(new ElectricCar(3, 6, true, true, 95, 12m));
        manager.AddVehicle(new Bicycle(4, 8, true, true));
        manager.AddItem(new ChargingStation(5, 3.75m));
        manager.AddItem(new Helmet(6));
        manager.AddItem(new FirstAidKit(7));

        Assert.Equal(19.5m, manager.GetEnergyConsumption());
    }

    [Fact]
    public void GetEnergyConsumption_RejectedVehicleDoesNotContribute() {
        var manager = new InventoryManager(new CheckVehicle());
        manager.AddVehicle(new ElectricScooter(1, 8, true, true, 29, 99m));
        manager.AddItem(new ChargingStation(2, 1.5m));

        Assert.Equal(1.5m, manager.GetEnergyConsumption());
        Assert.Equal(1, manager.GetInventoryReport().TotalCount);
    }

    [Fact]
    public void GetEnergyConsumption_EmptyPark_ReturnsZero() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        Assert.Equal(0m, manager.GetEnergyConsumption());
    }

    [Fact]
    public void GetBeginnerVehicles_UsesInclusiveSixBoundary() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        manager.AddVehicle(new Bicycle(1, 5, true, true));
        manager.AddVehicle(new Bicycle(2, 6, true, true));
        manager.AddVehicle(new ElectricScooter(3, 10, true, true, 40, 3m));
        manager.AddItem(new Helmet(4));

        var beginners = manager.GetBeginnerVehicles();

        Assert.Equal(2, beginners.Count);
        Assert.Contains(beginners, vehicle => vehicle.Id == 2);
        Assert.Contains(beginners, vehicle => vehicle.Id == 3);
        Assert.DoesNotContain(beginners, vehicle => vehicle.Id == 1);
    }

    [Fact]
    public void GetBeginnerVehicles_ExcludesTechnicallyRejectedVehicles() {
        var manager = new InventoryManager(new CheckVehicle());
        manager.AddVehicle(new ElectricCar(1, 10, true, true, 15, 20m));
        manager.AddVehicle(new Bicycle(2, 7, true, true));

        var beginners = manager.GetBeginnerVehicles();

        Assert.Single(beginners);
        Assert.Equal(2, beginners[0].Id);
    }
}
