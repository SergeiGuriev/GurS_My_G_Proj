using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health enemyHealth = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;
        void Update()
        {
            // это для того чтоб (float)0.3 не посчитало как (int)0.3
            if (Mathf.Approximately(enemyHealth.GetFraction(), 0) || Mathf.Approximately(enemyHealth.GetFraction(), 1))
            {
                canvas.enabled = false;
                return;
            }
            canvas.enabled = true;
            foreground.localScale = new Vector3(enemyHealth.GetFraction(), 1, 1);
        }
    }
}
