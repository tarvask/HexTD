namespace Match.Field.Tower
{
    public enum TowerState : byte
    {
        Undefined = 0,
        Placing = 1,
        Constructing = 2,
        ToRelease = 3,
        Active = 4,
        Removing = 5,
        ToDispose = 6
    }
}