namespace Vyshkat.Domain.Models;

/// <summary>Количество всего имущества</summary>
public sealed record InventoryReport(int VehicleCount, int ItemCount, int TotalCount);
