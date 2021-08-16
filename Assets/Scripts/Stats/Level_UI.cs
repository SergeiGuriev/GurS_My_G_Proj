using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class Level_UI : MonoBehaviour
    {
        BaseStats baseStats;
        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        private void Update()
        {
            GetComponent<Text>().text = baseStats.CalculateLvl().ToString();
        }
    }
}
