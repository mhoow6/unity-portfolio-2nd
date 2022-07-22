public abstract class InventoryItem
{
    public abstract void Use(int itemIndex, int count);

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
