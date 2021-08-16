using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnDelay = 4f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Fighter>().EquipWeapon(weapon);
                //Destroy(gameObject);
                StartCoroutine(RespawnWeaponAgain(respawnDelay));
            }
        }
        IEnumerator RespawnWeaponAgain(float delay)
        {
            ShowWeapon(false);
            yield return new WaitForSeconds(delay);
            ShowWeapon(true);
        }
        void ShowWeapon(bool isVisible)
        {
            GetComponent<Collider>().enabled = isVisible;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isVisible);
            }
        }
    }
}
