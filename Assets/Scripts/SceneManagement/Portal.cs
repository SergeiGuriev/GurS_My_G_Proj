using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

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
            DontDestroyOnLoad(gameObject); // не уничтожай сразу объект, вызвавший корутину, с прошлой сцены
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(nextLvlName);  // если загрузить лвл будет тяжело то fader.FadeOut сделает экран белым до тех пор пока 
                                                                    // не отработает LoadSceneAsync и только потом запустится FadeIn

            Portal nextPortal = GetNextPortal();
            UpdatePlayerPosition(nextPortal);
            yield return new WaitForSeconds(fadeWaitTime);  // Fader Canvas Group Alpha будет fadeWaitTime секунд 1(белым)
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
            // yield return сработает только ПОСЛЕ ПОЛНОЙ АСИНХРОННОЙ ЗАГРУЗКИ уровня.
            // делаем это асинхронно потому что обычный LoadScene пытается загрузить уровень в течение ОДНОГО следующего фрейма
            // если уровень будет "тяжёлым" то возможно будет работать звук, но картинка и сама программа/игра зафризится
            // до того момента пока всё не прогрузится(при обычном LoadScene)
        }

        private void UpdatePlayerPosition(Portal nextPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            //player.transform.position = nextPortal.portalSpawnPoint.position;
            player.GetComponent<NavMeshAgent>().Warp(nextPortal.portalSpawnPoint.position);
            player.transform.rotation = nextPortal.portalSpawnPoint.rotation;
        }

        private Portal GetNextPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;           // это portal, DontDestroyOnLoad(gameObject), из предыдущей сцены
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }
    }
}
