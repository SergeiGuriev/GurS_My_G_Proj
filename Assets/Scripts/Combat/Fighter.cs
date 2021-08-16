using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacs = 1f;
        [SerializeField] Transform rightHandWeaponPosition = null;
        [SerializeField] Transform leftHandWeaponPosition = null;
        [SerializeField] Weapon defWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;     // для того чтоб в if (timeSinceLastAttack > timeBetweenAttacs) ПЕРЕД ПЕРВЫМ УДАРОМ всегда было true
        private Weapon cWeapon;

        private void Start()
        {
            if (cWeapon == null)
            {
                EquipWeapon(defWeapon);
            }
        }


        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        public void EquipWeapon(Weapon weapon)
        {
            cWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.SpawnWeapon(rightHandWeaponPosition, leftHandWeaponPosition, animator);
        }

        // Unarmed animation trigger
        void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacs)
            {
                StartAttackTrigger();
                timeSinceLastAttack = 0;
            }
        }

        private void StartAttackTrigger()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event Hit
        private void Hit()
        {
            if (target == null) return;
            int damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (cWeapon.HasProjectile())
            {
                cWeapon.LaunchProjectile(rightHandWeaponPosition, leftHandWeaponPosition, target, gameObject, damage);
            }
            else
            {
                //target.TakeDamage(gameObject, cWeapon.GetWeaponDamage());                
                target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot()
        {
            Hit();
        }
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < cWeapon.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health target = combatTarget.GetComponent<Health>();
            return target != null && !target.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttackTrigger();
            target = null;
        }

        private void StopAttackTrigger()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public object CaptureState()
        {
            return cWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }

        public Health GetTarget()
        {
            return target;
        }
    }
}
