using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        Collider colider;
        private void Start()
        {
            health = GetComponent<Health>();
            colider = GetComponent<Collider>();            
        }
        void Update()
        {
            colider.enabled = !health.IsDead();
            if (health.IsDead()) return;
            if (InteractWithCombat()) return;
            if(InteractWithMovement()) return;
        }
        
        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();   // просто проверяем есть ли наличие такого компонента на объекте.
                if (target == null) continue;
                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);                    
                }
                return true;
            }
            return false;
        }
        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}