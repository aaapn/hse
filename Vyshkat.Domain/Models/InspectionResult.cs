namespace Vyshkat.Domain.Models;

/// <summary>Решение техосмотра</summary>
public sealed class InspectionResult
{
    // Denied пусто при успехе
    public InspectionResult(bool approved, string denied)
    {
        ArgumentNullException.ThrowIfNull(denied); 
        if (approved && denied.Length > 0) {
            throw new ArgumentException("При успешном техосмотре причина отказа должна быть пустой.", nameof(denied));
        }
        if (!approved && string.IsNullOrWhiteSpace(denied)) {
            throw new ArgumentException("При отказе нужна причина.", nameof(denied));
        }

        Approved = approved;
        Denied = denied;
    }

    public bool Approved { get; }
    public string Denied { get; }
}
