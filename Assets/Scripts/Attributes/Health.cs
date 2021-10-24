using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }
        private float health = -1f;
        private bool isDead;

        private void Start()
        {
            if (health < 0)
            {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLvlUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLvlUp -= RegenerateHealth;
        }

        private void RegenerateHealth()
        {
            health = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            health = Mathf.Max(health - damage, 0);
            if (health == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.IncrExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if (health == 0)
            {
                Die();
            }
        }

        public float GetHealthPercentage()
        {
            return (health * 100) / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetFraction()
        {
            return health / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Heal(float healthRestorePoints)
        {
            health = Mathf.Min(health + healthRestorePoints, GetMaxHealth());
        }
    }
}