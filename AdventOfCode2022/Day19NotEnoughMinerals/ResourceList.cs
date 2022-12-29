namespace Day19NotEnoughMinerals;

internal record struct ResourceList(
    ushort Ore = 0,
    ushort Clay = 0,
    ushort Obsidian = 0,
    ushort Geode = 0)
{
    public ushort this[Resource resource]
    {
        get
        {
            return resource switch
            {
                Resource.Ore => Ore,
                Resource.Clay => Clay,
                Resource.Obsidian => Obsidian,
                _ => Geode
            };
        }
    }

    public ResourceList Set(Resource resource, ushort amount)
    {
        return resource switch
        {
            Resource.Ore => new ResourceList(amount, Clay, Obsidian, Geode),
            Resource.Clay => new ResourceList(Ore, amount, Obsidian, Geode),
            Resource.Obsidian => new ResourceList(Ore, Clay, amount, Geode),
            _ => new ResourceList(Ore, Clay, Obsidian, amount),
        };
    }

    public ResourceList Add(Resource resource, short amount)
    {
        return resource switch
        {
            Resource.Ore => new ResourceList((ushort)(Ore + amount), Clay, Obsidian, Geode),
            Resource.Clay => new ResourceList(Ore, (ushort)(Clay + amount), Obsidian, Geode),
            Resource.Obsidian => new ResourceList(Ore, Clay, (ushort)(Obsidian + amount), Geode),
            _ => new ResourceList(Ore, Clay, Obsidian, (ushort)(Geode + amount)),
        };
    }

    public static ResourceList operator +(ResourceList left, ResourceList right)
    {
        return new ResourceList(
            (ushort)(left.Ore + right.Ore),
            (ushort)(left.Clay + right.Clay),
            (ushort)(left.Obsidian + right.Obsidian),
            (ushort)(left.Geode + right.Geode));
    }

    public static ResourceList operator -(ResourceList left, ResourceList right)
    {
        return new ResourceList(
            (ushort)(left.Ore - right.Ore),
            (ushort)(left.Clay - right.Clay),
            (ushort)(left.Obsidian - right.Obsidian),
            (ushort)(left.Geode - right.Geode));
    }
}
