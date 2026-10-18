using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Models;

namespace Vyshkat.Domain.Services;

/// <summary>Единая постановка на баланс, выборки и отчёты</summary>
public sealed class InventoryManager {
    private readonly List<InventoryItem> _items = new();
    private readonly ServiceCenter _serviceCenter;

    public InventoryManager(ServiceCenter serviceCenter) {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public OperationResult AddVehicle(Vehicle vehicle) {
        ArgumentNullException.ThrowIfNull(vehicle);

        if (HasId(vehicle.Id)) {
            return new OperationResult(false, $"Объект с ID {vehicle.Id} уже существует.");
        }

        InspectionResult inspection = _serviceCenter.Inspect(vehicle);
        if (!inspection.Approved) {
            return new OperationResult(false, $"Транспорт не принят в парк. Причина: {inspection.Denied}");
        }

        _items.Add(vehicle);
        return new OperationResult(true, $"{vehicle.TypeName} с ID {vehicle.Id} принят в парк.");
    }

    public OperationResult AddItem(InventoryItem item){
        ArgumentNullException.ThrowIfNull(item);

        // Обязательный техосмотр для любого транспорта
        if (item is Vehicle) return new OperationResult(false, "Для транспорта используйте AddVehicle и техосмотр.");

        if (HasId(item.Id)) return new OperationResult(false, $"Объект с ID {item.Id} уже существует.");

        _items.Add(item);
        return new OperationResult(true, $"{item.TypeName} с ID {item.Id} добавлен на баланс.");
    }

    public IReadOnlyList<IInventory> GetItems() => _items.ToArray();

    public IReadOnlyList<Vehicle> GetBeginnerVehicles() => _items.OfType<Vehicle>().Where(vehicle => vehicle.SuitableForBeginners).ToArray();

    public InventoryReport GetInventoryReport() {
        int vehicleCount = _items.Count(item => item is Vehicle);
        return new InventoryReport(vehicleCount, _items.Count - vehicleCount, _items.Count);
    }

    public decimal GetEnergyConsumption() => _items.OfType<EnergyConsumer>().Sum(item => item.EnergyConsumption);

    private bool HasId(int id) => _items.Any(item => item.Id == id);
}
