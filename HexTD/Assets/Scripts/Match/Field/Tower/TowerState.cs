namespace Match.Field.Tower
{
    public enum TowerState : byte
    {
        Undefined = 0,
        Constructing = 1,
        ToRelease = 2,
        Active = 3,
        Removing = 4,
        ToDispose = 5
    }
}