using UnityEngine;

namespace AbilitySystem.Abilitys
{
    [RequireComponent(typeof(Collider))]
    public class OilBehavior : MonoBehaviour
    {
        [SerializeField] private float _modifire;
        [SerializeField] private float _duration;

        [SerializeField] private Collider _collider;

        private void Update()
        {
            _duration -= Time.deltaTime;

            if (_duration < 0)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CarController carController))
            {
                //add logic for slow
            }
        }
    }
}