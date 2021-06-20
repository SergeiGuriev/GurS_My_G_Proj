using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]          // автоматом подтянет Health компонент и не даст удалить его без удаления CombatTarget
    public class CombatTarget : MonoBehaviour   // способ избежания ошибок и забывчивости
    {

    }
}