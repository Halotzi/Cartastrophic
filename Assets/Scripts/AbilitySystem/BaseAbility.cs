namespace AbilitySystem
{
    public abstract class BaseAbility
    {
        public string AbilityName { get; protected set; }
        public AbilityCaster Caster { get; set; }

        protected BaseAbility(AbilityCaster caster)
        {
            Caster = caster;
        }
        
        public abstract void Cast();
    }
}