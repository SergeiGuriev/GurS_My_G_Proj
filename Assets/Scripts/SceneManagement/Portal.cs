using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }
        [SerializeField] string nextLvlName = null;
        [SerializeField] Transform portalSpawnPoint = null;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine("SceneLoad");
            }
        }

        IEnumerator SceneLoad()
        {            
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();

            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;


            yield return fader.FadeOut(fadeOutTime);
            // save lvl
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            // для этого сейва обязательно должен отработать лоад, а потом сделать сохр этого же сейва.
            // похоже, что нельзя давать этим 2 сейвам разные ключи!!!!! и это всё работа над одним сейвом, а не 2 отдельных

            // можно попробовать отсюда кидать bool saveFromPortal = true
            // и в сейвврапере проверять на тру, если тру то выполнять обработку по другому
            // в других местах передавай false

            wrapper.Save(true);//true
            yield return SceneManager.LoadSceneAsync(nextLvlName);

            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            // load lvl
            wrapper.Load(true);//true
            Portal nextPortal = GetNextPortal();
            UpdatePlayerPosition(nextPortal);
            wrapper.Save(true);//true
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

        private void UpdatePlayerPosition(Portal nextPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(nextPortal.portalSpawnPoint.position);
            player.transform.rotation = nextPortal.portalSpawnPoint.rotation;
        }

        private Portal GetNextPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }
    }
}
