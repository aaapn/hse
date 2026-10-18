namespace Vyshkat.Domain.Models;

public sealed class FirstAidKit : InventoryItem
{
    public FirstAidKit(int id) : base(id) { }

    public override string TypeName => "Аптечка первой помощи";
}
