using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]          // автоматом подтянет Health компонент и не даст удалить его без удаления CombatTarget
    public class CombatTarget : MonoBehaviour   // способ избежания ошибок и забывчивости
    {

    }
}