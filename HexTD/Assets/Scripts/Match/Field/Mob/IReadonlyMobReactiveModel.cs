using BuffLogic;

namespace Match.Field.Mob
{
    public interface IReadonlyMobReactiveModel
    {
        IReadonlyBuffableValue<float> Health { get; }
        IReadonlyBuffableValue<float> Speed { get; }
    }
}