using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPref = null;
        public void Spawn(float damage)
        {
            DamageText instance = Instantiate<DamageText>(damageTextPref, transform);
            instance.SetValue(damage);
        }
    }
}
