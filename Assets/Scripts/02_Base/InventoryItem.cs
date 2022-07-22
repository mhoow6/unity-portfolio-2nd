public abstract class InventoryItem
{
    public abstract void Use(int itemIndex);

    public static InventoryItem Get(ItemType type)
    {
        switch (type)
        {
            case ItemType.EnergyRefill:
                return new EnergyRefillItem();
            case ItemType.GoldRefill:
                return new GoldRefillItem();
        }
        return null;
    }
}
