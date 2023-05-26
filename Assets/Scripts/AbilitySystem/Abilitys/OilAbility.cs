using UnityEngine;

namespace AbilitySystem.Abilitys
{
    public class OilAbility : BaseAbility
    {
        private GameObject _oilPrefab;
        
        public OilAbility(GameObject oilPrefab, AbilityCaster caster) : base(caster)
        {
            _oilPrefab  = oilPrefab;
        }

        public override void Cast()
        {
            var oil = Object.Instantiate(_oilPrefab, Caster.transform.position, Quaternion.identity);
        }
    }
}