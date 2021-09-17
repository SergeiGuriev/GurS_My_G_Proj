using RPG.Attributes;
using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class Enemy_Health_UI : MonoBehaviour
    {
        Fighter fighter;
        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            Health health = fighter.GetTarget();
            GetComponent<Text>().text = string.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
        }
    }
}
