using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class Experience_UI : MonoBehaviour
    {
        Experience experience;
        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }
        private void Update()
        {
            GetComponent<Text>().text = experience.GetXpVal().ToString();
        }
    }
}
