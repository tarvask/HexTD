namespace Match.Field.Castle
{
    public struct HealthInfo
    {
        public readonly int CurrentHealth;
        public readonly int MaxHealth;

        public HealthInfo(int currentHealth, int maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }
}