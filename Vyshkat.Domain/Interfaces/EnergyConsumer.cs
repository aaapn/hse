namespace Vyshkat.Domain.Interfaces;

/// <summary>Объект, потребляющий электричество за сутки</summary>
public interface EnergyConsumer
{
    decimal EnergyConsumption { get; }
}
