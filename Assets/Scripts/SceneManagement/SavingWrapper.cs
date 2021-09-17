﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFileName = "save";
        [SerializeField] float fadeInTime = 0.2f;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }
       
        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(saveFileName);
            yield return fader.FadeIn(fadeInTime);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(saveFileName);
        }
        public void Save()
        {
            GetComponent<SavingSystem>().Save(saveFileName);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(saveFileName);
        }
    }
}
