namespace Vyshkat.Domain.Models;

/// <summary>Итог операции постановки имущества на баланс</summary>
public sealed class OperationResult {
    public OperationResult(bool success, string message) {
        if (string.IsNullOrWhiteSpace(message)) {
            throw new ArgumentException("Сообщение не должно быть пустым.", nameof(message));
        }

        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string Message { get; }
}
