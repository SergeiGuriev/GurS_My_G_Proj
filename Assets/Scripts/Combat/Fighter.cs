using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacs = 1f;
        [SerializeField] Transform rightHandWeaponPosition = null;
        [SerializeField] Transform leftHandWeaponPosition = null;
        [SerializeField] Weapon defWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        private Weapon cWeapon;

        private void Awake()
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
            if (!GetIsInRange(target.transform))
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

        private void Hit()
        {
            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (cWeapon.HasProjectile())
            {
                cWeapon.LaunchProjectile(rightHandWeaponPosition, leftHandWeaponPosition, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot()
        {
            Hit();
        }
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < cWeapon.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return cWeapon.GetWeaponDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return cWeapon.GetPercentageBonus();
            }
        }

        public float GetCDamage()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Damage);
        }
    }
}
