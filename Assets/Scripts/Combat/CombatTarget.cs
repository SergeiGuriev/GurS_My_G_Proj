using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]          // автоматом подтянет Health компонент и не даст удалить его без удаления CombatTarget
    public class CombatTarget : MonoBehaviour, IRaycastable   // способ избежания ошибок и забывчивости
    {       

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject)) return false; // Если НЕатакуемый
            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}