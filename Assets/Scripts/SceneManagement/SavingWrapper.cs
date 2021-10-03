using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private string saveFileName;
        public List<string> savesArray;


        [SerializeField] float fadeInTime = 0.2f;


        bool isNoDataSceneLoaded = false;
        static string tempSaveName;
        private void Update()
        {
            //=======================================================
            // ====================ОСТОРОЖНО!!!
            //=======================================================


            
            if (PlayerPrefs.GetString("isLoadButtonPressed") == "true")
            {
                if (isNoDataSceneLoaded)
                {
                    isNoDataSceneLoaded = false;
                    PlayerPrefs.SetString("isLoadButtonPressed", "false");
                    Load(false, tempSaveName, false);     // впихнуть данные
                }
                else
                {
                    isNoDataSceneLoaded = true;
                    // нужно грузить сцену на которую записано сохранение
                    tempSaveName = PlayerPrefs.GetString("csElseEnteredSaveName");
                    GameObject PO = GameObject.Find("Player");
                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;
                    SceneManager.LoadScene(requiredScene);       // загрузить сцену БЕЗ ДАННЫХ
                    //SceneManager.LoadScene(1);       // загрузить сцену БЕЗ ДАННЫХ
                }
            }



            if (PlayerPrefs.GetString("isSceneLoaded") == "true")
            {
                PlayerPrefs.SetString("isSceneLoaded", "false");
                if (PlayerPrefs.GetString("isFromMenu") == "true")
                {
                    PlayerPrefs.SetString("isFromMenu", "false");
                    print("PlayerPrefsSaveName = " + PlayerPrefs.GetString("saveName"));
                    Load(false, PlayerPrefs.GetString("saveName"), false);
                }
                else
                {
                    Load(false, null, false);
                }
                //PlayerPrefs.SetString("saveName", null);
            }



            if (Input.GetKeyDown(KeyCode.L))
            {
                Load(false, null, false);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save(false);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete(null, -1);
            }
        }


        IEnumerator Start()
        {
            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
            if (arrCount != 0)
            {
                savesArray = new List<string>();
                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
                {
                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
                }
            }
            else
            {
                savesArray = new List<string>();

                //// во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
                //for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
                //{
                //    PlayerPrefs.DeleteKey(i.ToString());
                //}
                //PlayerPrefs.DeleteKey("currentArrayCount");
            }

            Fader fader = FindObjectOfType<Fader>();
            //
            if (fader != null)
            {
                fader.FadeOutImmediate();
                if (savesArray.Any())
                {
                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); // после первого запуска загрузится последний сейв
                }
                else
                {
                    yield return GetComponent<SavingSystem>().LoadLastScene("save");    // или это если нету сохранений
                }
                yield return fader.FadeIn(fadeInTime);
            }
        }



        public void Save(bool isFromPortal)
        {
            if (!isFromPortal)
            {
                saveFileName = string.Format(@"{0}", Guid.NewGuid());
                //------------------------------------
                // делаем unset для старого списка
                UnsetPlayerPrefsListIndexes(savesArray);
                //------------------------------------

                savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

                //------------------------------------
                // сохраняем в PlayerPrefs все индексы нового списка
                for (int i = 0; i < savesArray.Count; i++)
                {
                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
                }
                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
                //------------------------------------

                GetComponent<SavingSystem>().Save(saveFileName);
            }
            else
            {
                GetComponent<SavingSystem>().Save("portalSave");
            }
        }



        public void Load(bool isFromPortal, string saveName, bool isLoadButtonClick)
        {
            PlayerPrefs.SetString("saveName", "null");
            PlayerPrefs.SetString("csElseEnteredSaveName", "null");
            if (!isFromPortal)
            {
                if (savesArray.Any())
                {
                    if (saveName == null)
                    {
                        GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для загрузки("L") - загрузи последнее
                    }
                    else
                    {
                        GetComponent<SavingSystem>().Load(saveName);        // тут просто вытягиваются данные, не перезагр сцену
                        if (isLoadButtonClick)
                        {
                            PlayerPrefs.SetString("isLoadButtonPressed", "true");
                            PlayerPrefs.SetString("csElseEnteredSaveName", saveName);
                            SceneManager.LoadScene(0);
                        }
                    }

                    //===============================================================================
                    // возможно в старте придётся делать похожее если не будет нормально отрабатывать
                    // имя файла savesArray[savesArray.Count - 1]
                    //===============================================================================
                    GameObject PO = GameObject.Find("Player");
                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;

                    // если текущая сцена != сцене из сохранения
                    if (SceneManager.GetActiveScene().buildIndex != requiredScene)
                    {
                        //ОСТОРОЖНО!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        PlayerPrefs.SetString("isSceneLoaded", "true");
                        if (saveName != null)
                        {
                            PlayerPrefs.SetString("isFromMenu", "true");

                            PlayerPrefs.SetString("saveName", saveName);
                        }
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        SceneManager.LoadScene(requiredScene);
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //если грузишься через UI
                        // в старте отрабатывает Load(false, null);     и грузит последнее сохранение
                        // а должно отрабатать Load(false, saveName);
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    }
                }
                else
                {
                    GetComponent<SavingSystem>().Load("save");
                }
            }
            else
            {
                GetComponent<SavingSystem>().Load("portalSave");
            }
        }


        public void Delete(string saveName, int saveIndex)
        {
            if (savesArray.Any())
            {
                if (saveName == null)
                {
                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для удаления - удали последнее
                    savesArray.RemoveAt(savesArray.Count - 1);
                    // делаю unset удалённого элемента
                    PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
                }
                else
                {
                    GetComponent<SavingSystem>().Delete(saveName);

                    // нужно сместить массив PlayerPrefs'ов и потом последний удалится ниже(PlayerPrefs.DeleteKey)
                    // (0,1,2) => (0,2,2)
                    if (saveIndex != 0)
                    {
                        for (int i = savesArray.Count - 1; i >= saveIndex; i--)
                        {
                            PlayerPrefs.SetString(i.ToString(), savesArray[i - 1]);
                            //savesArray[i] = savesArray[i - 1];
                        }
                    }
                    savesArray.RemoveAt(saveIndex);                                     ///////////////// удалил из списка
                    // делаю unset удалённого элемента
                    PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());           ///////////////// удалил из PlayerPrefs
                }


                if (savesArray.Count == 0)
                {
                    PlayerPrefs.DeleteKey("currentArrayCount");
                    GetComponent<SavingSystem>().Delete("portalSave");
                }
            }
            else
            {
                GetComponent<SavingSystem>().Delete("save");
            }
        }

        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
        {
            if (savesArray.Any())
            {
                for (int i = 0; i < savesArray.Count; i++)
                {
                    PlayerPrefs.DeleteKey(i.ToString());
                }
                PlayerPrefs.DeleteKey("currentArrayCount");
            }
        }

        // метод используй как ивент
        public void DropOldSaves()
        {
            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

            if (isCharacterSelected && isSceneChanged)
            {
                PrepareOldSaves();
            }
        }

        private void PrepareOldSaves()
        {
            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
            if (arrCount != 0)
            {
                savesArray = new List<string>();
                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
                {
                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
                }
            }

            if (savesArray != null)
            {
                int counter = savesArray.Count;
                // делаю unset
                UnsetPlayerPrefsListIndexes(savesArray);

                for (int i = 0; i < counter; i++)
                {
                    Debug.Log(savesArray[savesArray.Count - 1]);

                    //----------------
                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
                    //----------------
                    savesArray.RemoveAt(savesArray.Count - 1);
                }
                GetComponent<SavingSystem>().Delete("portalSave");
            }
        }
    }
}












#region old
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        const string saveFileName = "save";
//        [SerializeField] float fadeInTime = 0.2f;


//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.L))
//            {
//                Load();
//            }
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                Save();
//            }
//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                Delete();
//            }
//        }

//        IEnumerator Start()
//        {
//            Fader fader = FindObjectOfType<Fader>();
//            fader.FadeOutImmediate();
//            yield return GetComponent<SavingSystem>().LoadLastScene(saveFileName);
//            yield return fader.FadeIn(fadeInTime);
//        }

//        public void Load()
//        {
//            GetComponent<SavingSystem>().Load(saveFileName);
//        }
//        public void Save()
//        {
//            GetComponent<SavingSystem>().Save(saveFileName);
//        }

//        public void Delete()
//        {
//            GetComponent<SavingSystem>().Delete(saveFileName);
//        }
//    }
//}
#endregion