namespace Vyshkat.Domain.Models;

public sealed class Helmet : InventoryItem
{
    public Helmet(int id) : base(id) { }

    public override string TypeName => "Шлем";
}
