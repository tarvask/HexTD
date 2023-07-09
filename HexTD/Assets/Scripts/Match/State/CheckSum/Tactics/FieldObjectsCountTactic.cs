using Match.Field.State;

namespace Match.State.CheckSum
{
    public class FieldObjectsCountTactic : AbstractMatchStateCheckSumComputeTactic
    {
        private const int ProjectilesMaxCount = 256;
        private const int MobsMaxCount = 256;
        private const int TowersMaxCount = TestMatchEngine.TowersInHandCount;

        protected override int GetFieldCheckSum(in PlayerState fieldState)
        {
            int fieldCheckSum = 0;
            
            // projectiles
            int magnitudeOrder = 1;
            fieldCheckSum += fieldState.Projectiles.Projectiles.Length;
            
            // mobs
            magnitudeOrder *= ProjectilesMaxCount;
            fieldCheckSum += magnitudeOrder * fieldState.Mobs.Mobs.Length;
            
            // towers
            magnitudeOrder *= MobsMaxCount;
            fieldCheckSum += magnitudeOrder * fieldState.Towers.Towers.Length;

            return fieldCheckSum;
        }
    }
}