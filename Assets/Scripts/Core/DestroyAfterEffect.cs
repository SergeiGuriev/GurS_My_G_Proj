using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject destroyParent = null;
        void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (destroyParent != null)
                {
                    Destroy(destroyParent);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
