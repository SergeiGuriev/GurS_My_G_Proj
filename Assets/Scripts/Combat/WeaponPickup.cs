using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float healthRestorePoints = 0;
        [SerializeField] float respawnDelay = 4f;

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Pickup(callingController.GetComponent<Fighter>());
                callingController.GetComponent<Mover>().StartMoveAction(callingController.target);
            }
            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject picupObj)
        {
            if (weapon != null)
            {
                picupObj.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthRestorePoints > 0)
            {
                picupObj.GetComponent<Health>().Heal(healthRestorePoints);
            }
            StartCoroutine(RespawnWeaponAgain(respawnDelay));
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
