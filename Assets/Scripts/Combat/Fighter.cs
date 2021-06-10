using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacs = 1f;
        [SerializeField] int weaponDamage = 5;

        Transform target;
        float timeSinceLastAttack = 0;
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;            
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        // Unarmed animation trigger
        void AttackBehaviour()
        {
            if (timeSinceLastAttack > timeBetweenAttacs)
            {
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;                
            }
        }

        // Animation Event Hit from unarmed animation
        private void Hit()
        {
            Health targetObj = target.GetComponent<Health>();
            targetObj.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }

    }
}
