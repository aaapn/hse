using Microsoft.Extensions.DependencyInjection;
using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Models;
using Vyshkat.Domain.Services;
using Xunit;

namespace Vyshkat.Tests;

public sealed class InventoryManagerTests {
    [Fact]
    public void AddVehicle_ApprovedByFakeRegisteredInDi_AddsVehicle() {
        var fake = new FakeServiceCenter(approved: true);
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<ServiceCenter>(fake)
            .AddSingleton<InventoryManager>()
            .BuildServiceProvider();

        InventoryManager manager = provider.GetRequiredService<InventoryManager>();
        var scooter = new ElectricScooter(101, 5, true, true, 10, 3m);

        OperationResult result = manager.AddVehicle(scooter);

        Assert.True(result.Success);
        Assert.Single(manager.GetItems());
        Assert.Equal(1, fake.InspectionCount);
    }

    [Fact]
    public void AddVehicle_RejectedByFakeRegisteredInDi_DoesNotAddVehicle() {
        var fake = new FakeServiceCenter(approved: false);
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<ServiceCenter>(fake)
            .AddSingleton<InventoryManager>()
            .BuildServiceProvider();

        InventoryManager manager = provider.GetRequiredService<InventoryManager>();

        OperationResult result = manager.AddVehicle(new Bicycle(102, 7, true, true));

        Assert.False(result.Success);
        Assert.Contains("Отказ тестового техосмотра", result.Message);
        Assert.Empty(manager.GetItems());
        Assert.Equal(1, fake.InspectionCount);
    }

    [Fact]
    public void AddVehicle_ExistingThingId_RejectsWithoutInspection() {
        var fake = new FakeServiceCenter(true);
        var manager = new InventoryManager(fake);
        Assert.True(manager.AddItem(new Helmet(77)).Success);

        OperationResult result = manager.AddVehicle(new Bicycle(77, 6, true, true));

        Assert.False(result.Success);
        Assert.Contains("77", result.Message);
        Assert.Equal(0, fake.InspectionCount);
        Assert.Single(manager.GetItems());
    }

    [Fact]
    public void AddItem_ExistingVehicleId_RejectsDuplicate() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        Assert.True(manager.AddVehicle(new Bicycle(55, 6, true, true)).Success);

        OperationResult result = manager.AddItem(new FirstAidKit(55));

        Assert.False(result.Success);
        Assert.Single(manager.GetItems());
    }

    [Fact]
    public void AddItem_ExistingThingId_RejectsDuplicate() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        Assert.True(manager.AddItem(new Helmet(99)).Success);

        OperationResult result = manager.AddItem(new ChargingStation(99, 1.2m));

        Assert.False(result.Success);
        Assert.Single(manager.GetItems());
    }

    [Fact]
    public void AddItem_TransportPassedAsThing_CannotBypassInspection() {
        var fake = new FakeServiceCenter(true);
        var manager = new InventoryManager(fake);

        OperationResult result = manager.AddItem(new Bicycle(12, 8, true, true));

        Assert.False(result.Success);
        Assert.Contains("AddVehicle", result.Message);
        Assert.Empty(manager.GetItems());
        Assert.Equal(0, fake.InspectionCount);
    }

    [Fact]
    public void AddVehicle_RejectedIdCanBeReusedLater() {
        var fake = new FakeServiceCenter(false);
        var manager = new InventoryManager(fake);
        Assert.False(manager.AddVehicle(new Bicycle(11, 5, true, true)).Success);
        fake.Approved = true;

        OperationResult result = manager.AddVehicle(new Bicycle(11, 5, true, true));

        Assert.True(result.Success);
        Assert.Single(manager.GetItems());
        Assert.Equal(2, fake.InspectionCount);
    }

    [Fact]
    public void GetItems_ReturnsCopyOfInventory() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        manager.AddItem(new Helmet(1));
        var snapshot = manager.GetItems();
        manager.AddItem(new FirstAidKit(2));

        Assert.Single(snapshot);
        Assert.Equal(2, manager.GetItems().Count);
    }

    [Fact]
    public void AddVehicle_Null_Throws() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        Assert.Throws<ArgumentNullException>(() => manager.AddVehicle(null!));
    }

    [Fact]
    public void AddItem_Null_Throws() {
        var manager = new InventoryManager(new FakeServiceCenter(true));
        Assert.Throws<ArgumentNullException>(() => manager.AddItem(null!));
    }

    [Fact]
    public void Constructor_NullServiceCenter_Throws() {
        Assert.Throws<ArgumentNullException>(() => new InventoryManager(null!));
    }
}
