using Vyshkat.Domain.Models;

namespace Vyshkat.Domain.Interfaces;

/// <summary>Техосмотр транспорта до постановки на баланс</summary>
public interface ServiceCenter
{
    InspectionResult Inspect(Vehicle vehicle);
}
