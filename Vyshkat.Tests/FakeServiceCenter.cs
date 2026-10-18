using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Models;

namespace Vyshkat.Tests;

/// <summary>Подмена техосмотра только для тестов</summary>
public sealed class FakeServiceCenter : ServiceCenter {
    public FakeServiceCenter(bool approved) => Approved = approved;

    public bool Approved { get; set; }
    public int InspectionCount { get; private set; }

    public InspectionResult Inspect(Vehicle vehicle) {
        InspectionCount++;
        return Approved ? new InspectionResult(true, string.Empty) : new InspectionResult(false, "Отказ тестового техосмотра.");
    }
}
