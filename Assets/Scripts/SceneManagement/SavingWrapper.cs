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





            //if (PlayerPrefs.GetString("csElseEntered") == "true")
            //{
            //    // баг когда лоад данных выполнился, а сцена как будто не перезагрузилась(мёртвые враги не встают даже на тех сохранениях где у них было хп)
            //    GetComponent<SavingSystem>().Load(PlayerPrefs.GetString("csElseEnteredSaveName"));                  // ПОЧЕМУ НЕ ОТРАБАТЫВАЕТ??????!!!!!!!!!!
            //    PlayerPrefs.SetString("csElseEntered", "false");
            //    Debug.Log("ЫЫЫЫЫЫЫЫЫЫЫЫЫЫ!!!!!!!!!!!!!!!!!!!" + PlayerPrefs.GetString("csElseEnteredSaveName"));
            //}
            //========================================================

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
                    //else
                    //{
                    //    // поднять врагов
                    //    //PlayerPrefs.SetString("csElseEntered", "true");
                    //    //PlayerPrefs.SetString("csElseEnteredSaveName", saveName);
                    //    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    //}
                    //===============================================================================
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








//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        //const string saveFileName = "save";

//        private string saveFileName;
//        List<string> savesArray;


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
//            savesArray = new List<string>();

//            Fader fader = FindObjectOfType<Fader>();
//            fader.FadeOutImmediate();
//            //yield return GetComponent<SavingSystem>().LoadLastScene(saveFileName);
//            if (savesArray.Any())
//            {
//                yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); //нужно будет передавать индекс
//            }
//            else
//            {
//                yield return GetComponent<SavingSystem>().LoadLastScene("save");
//            }
//            yield return fader.FadeIn(fadeInTime);
//        }

//        public void Load()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);    // нужно будет передавать индекс
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("save");
//            }
//        }
//        public void Save()
//        {
//            saveFileName = string.Format(@"{0}", Guid.NewGuid());
//            savesArray.Add(saveFileName);

//            GetComponent<SavingSystem>().Save(saveFileName);
//            print(savesArray.Count);
//        }

//        public void Delete()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);      // нужно будет передавать индекс
//                savesArray.RemoveAt(savesArray.Count - 1);
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }
//    }
//}





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        private string saveFileName;
//        List<string> savesArray;


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


//        // сохранение в портале отрабатывает 2 раза. 
//        // и второй раз перезаписывает первую запись

//        IEnumerator Start()
//        {
//            if (PlayerPrefs.HasKey("currentArrayCount"))
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }
//            else
//            {
//                savesArray = new List<string>();

//                // во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }

//            Fader fader = FindObjectOfType<Fader>();
//            //
//            if (fader != null)
//            {
//                fader.FadeOutImmediate();
//                if (savesArray.Any())
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); //нужно будет передавать имя файла сохранения
//                }
//                else
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene("save");
//                }
//                yield return fader.FadeIn(fadeInTime);
//            }
//        }

//        public void Load()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);    // нужно будет передавать имя файла сохранения
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("save");
//            }
//        }
//        public void Save()//true/false from portal
//        {
//            //if пришло false
//            //{

//            //если первый раз отработал else то saveFileName нужно взять из else(бул добавь для проверки первого отрабатывания)
//            saveFileName = string.Format(@"{0}", Guid.NewGuid());
//            //------------------------------------
//            // делаем unset для старого списка
//            UnsetPlayerPrefsListIndexes(savesArray);
//            //------------------------------------

//            savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

//            //------------------------------------
//            // сохраняем в PlayerPrefs все индексы нового списка
//            for (int i = 0; i < savesArray.Count; i++)
//            {
//                PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//            }
//            PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//            //------------------------------------

//            GetComponent<SavingSystem>().Save(saveFileName);


//            //}
//            //else    if пришло true
//            //{
//            // два вызова должны будут отработать с одним файлом с одним ключом
//                //saveFileName = string.Format(@"{0}", Guid.NewGuid());
//            //}
//        }

//        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
//        {
//            if (savesArray.Any())
//            {
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }
//        }

//        public void Delete()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);      // нужно будет передавать имя файла сохранения
//                savesArray.RemoveAt(savesArray.Count - 1);
//                // делаю unset удалённого элемента
//                PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }


//        // метод используй как ивент
//        public void DropOldSaves()
//        {
//            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
//            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
//            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

//            if (isCharacterSelected && isSceneChanged)
//            {
//                PrepareOldSaves();
//            }
//        }

//        public void PrepareOldSaves()
//        {
//            if (savesArray.Any())
//            {
//                int counter = savesArray.Count;

//                // делаю unset
//                UnsetPlayerPrefsListIndexes(savesArray);

//                for (int i = 0; i < counter; i++)
//                {
//                    Debug.Log(savesArray[savesArray.Count - 1]);

//                    //----------------
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
//                    //----------------
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                }
//            }
//        }
//    }
//}
























//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        private string saveFileName;
//        List<string> savesArray;


//        [SerializeField] float fadeInTime = 0.2f;


//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.L))
//            {
//                Load(false);
//            }
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                Save(false);
//            }
//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                Delete();
//            }
//        }


//        // сохранение в портале отрабатывает 2 раза. 
//        // и второй раз перезаписывает первую запись

//        IEnumerator Start()
//        {
//            if (PlayerPrefs.HasKey("currentArrayCount"))
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }
//            else
//            {
//                savesArray = new List<string>();

//                // во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }

//            Fader fader = FindObjectOfType<Fader>();
//            //
//            if (fader != null)
//            {
//                fader.FadeOutImmediate();
//                if (savesArray.Any())
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); //нужно будет передавать имя файла сохранения
//                }
//                else
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene("save");
//                }
//                yield return fader.FadeIn(fadeInTime);
//            }
//        }

//        public void Load(bool isForPortal)
//        {
//            //if(true)...
//            //else (){ 
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);    // нужно будет передавать имя файла сохранения
//                Debug.Log("*Load: "+ savesArray[savesArray.Count - 1]);
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("save");
//            }
//        }
//        bool isEntered = false;
//        bool isEntered2 = false;
//        string entered2SaveFileName;
//        public void Save(bool isForPortal)//true/false from portal
//        {
//            if (!isForPortal)
//            {
//                //если первый раз отработал else то saveFileName нужно взять из else(бул добавь для проверки первого отрабатывания)
//                if (!isEntered)
//                {
//                    saveFileName = string.Format(@"{0}", Guid.NewGuid());
//                }
//                //------------------------------------
//                // делаем unset для старого списка
//                UnsetPlayerPrefsListIndexes(savesArray);
//                //------------------------------------


//                savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

//                //------------------------------------
//                // сохраняем в PlayerPrefs все индексы нового списка
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                }
//                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                //------------------------------------


//                #region deb
//                Debug.Log("-------if1-------");
//                GetComponent<SavingSystem>().Save(saveFileName);
//                Debug.Log("*Save if: " + saveFileName+ ", isEntered = "+ isEntered);
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    print(savesArray[i]);
//                }
//                Debug.Log("-------if1-------");
//                #endregion

//                if (isEntered)
//                {
//                    //удалить предпоследний файл сохранения
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 2]);  // удал файл

//                    // удал из списка предпосл элемент
//                    savesArray.RemoveAt(savesArray.Count - 2);

//                    // переписать ключи и ключ размерности списка
//                    UnsetPlayerPrefsListIndexes(savesArray);
//                    for (int i = 0; i < savesArray.Count; i++)
//                    {
//                        PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                    }
//                    PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                }

//                #region deb2
//                Debug.Log("-------if2-------");
//                GetComponent<SavingSystem>().Save(saveFileName);
//                Debug.Log("*Save if: " + saveFileName + ", isEntered = " + isEntered);
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    print("i = "+i+"save: "+savesArray[i]);
//                }
//                Debug.Log("-------if2-------");
//                #endregion
//                isEntered = false;
//            }
//            else
//            {
//                if (!isEntered2)
//                {
//                    //прошлый сейв, а не запуск того что ниже
//                    saveFileName = string.Format(@"{0}", Guid.NewGuid());
//                    entered2SaveFileName = saveFileName;
//                    isEntered2 = true;
//                }
//                else
//                {
//                    isEntered2 = false;
//                    saveFileName = entered2SaveFileName;
//                }

//                UnsetPlayerPrefsListIndexes(savesArray);
//                savesArray.Add(saveFileName);
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                }
//                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                GetComponent<SavingSystem>().Save(saveFileName);

//                isEntered = true;   // первый сейв, был заход

//                #region deb
//                Debug.Log("-------else-------");
//                Debug.Log("*Save else: " + saveFileName + ", isEntered = " + isEntered);
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    print(savesArray[i]);
//                }
//                Debug.Log("-------else-------");
//                #endregion
//            }

//            //}
//            //else    if пришло true
//            //{
//            // два вызова должны будут отработать с одним файлом с одним ключом
//            //saveFileName = string.Format(@"{0}", Guid.NewGuid());
//            //}
//        }

//        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
//        {
//            if (savesArray.Any())
//            {
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }
//        }

//        public void Delete()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);      // нужно будет передавать имя файла сохранения
//                savesArray.RemoveAt(savesArray.Count - 1);
//                // делаю unset удалённого элемента
//                PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }


//        // метод используй как ивент
//        public void DropOldSaves()
//        {
//            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
//            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
//            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

//            if (isCharacterSelected && isSceneChanged)
//            {
//                PrepareOldSaves();
//            }
//        }

//        public void PrepareOldSaves()
//        {
//            if (savesArray.Any())
//            {
//                int counter = savesArray.Count;

//                // делаю unset
//                UnsetPlayerPrefsListIndexes(savesArray);

//                for (int i = 0; i < counter; i++)
//                {
//                    Debug.Log(savesArray[savesArray.Count - 1]);

//                    //----------------
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
//                    //----------------
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                }
//            }
//        }
//    }
//}














//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;
//using UnityEngine.SceneManagement;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        private string saveFileName;
//        List<string> savesArray;


//        [SerializeField] float fadeInTime = 0.2f;


//        private void Update()
//        {
//            //=======================================================
//            // ====================ОСТОРОЖНО!!!
//            //=======================================================
//            if (PlayerPrefs.GetString("isSceneLoaded") == "true")
//            {
//                Load(false);
//                PlayerPrefs.SetString("isSceneLoaded", "false");
//            }
//            //========================================================

//            if (Input.GetKeyDown(KeyCode.L))
//            {
//                Load(false);
//            }
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                Save(false);
//            }
//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                Delete();
//            }
//        }


//        IEnumerator Start()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }
//            else
//            {
//                savesArray = new List<string>();

//                //// во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
//                //for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                //{
//                //    PlayerPrefs.DeleteKey(i.ToString());
//                //}
//                //PlayerPrefs.DeleteKey("currentArrayCount");
//            }

//            Fader fader = FindObjectOfType<Fader>();
//            //
//            if (fader != null)
//            {
//                fader.FadeOutImmediate();
//                if (savesArray.Any())
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); //нужно будет передавать имя файла сохранения
//                }
//                else
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene("save");
//                }
//                yield return fader.FadeIn(fadeInTime);
//            }
//        }



//        public void Save(bool isFromPortal)
//        {
//            if (!isFromPortal)
//            {
//                saveFileName = string.Format(@"{0}", Guid.NewGuid());
//                //------------------------------------
//                // делаем unset для старого списка
//                UnsetPlayerPrefsListIndexes(savesArray);
//                //------------------------------------

//                savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

//                //------------------------------------
//                // сохраняем в PlayerPrefs все индексы нового списка
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                }
//                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                //------------------------------------

//                GetComponent<SavingSystem>().Save(saveFileName);
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Save("portalSave");
//            }
//        }



//        public void Load(bool isFromPortal)
//        {
//            if (!isFromPortal)
//            {
//                if (savesArray.Any())
//                {
//                    GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);    // нужно будет передавать имя файла сохранения

//                    //===============================================================================
//                    // возможно в старте придётся делать похожее если не будет нормально отрабатывать
//                    // имя файла savesArray[savesArray.Count - 1]
//                    //===============================================================================
//                    GameObject PO = GameObject.Find("Player");
//                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;
//                    // если текущая сцена != сцене из сохранения
//                    if (SceneManager.GetActiveScene().buildIndex != requiredScene)
//                    {
//                        //ОСТОРОЖНО!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                        PlayerPrefs.SetString("isSceneLoaded", "true");
//                        SceneManager.LoadScene(requiredScene);
//                    }
//                    //===============================================================================
//                }
//                else
//                {
//                    GetComponent<SavingSystem>().Load("save");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("portalSave");
//            }
//        }


//        public void Delete()
//        {
//            if (savesArray.Any())
//            {
//                GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);      // нужно будет передавать имя файла сохранения
//                savesArray.RemoveAt(savesArray.Count - 1);
//                // делаю unset удалённого элемента
//                PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
//                if (savesArray.Count == 0)
//                {
//                    PlayerPrefs.DeleteKey("currentArrayCount");
//                    GetComponent<SavingSystem>().Delete("portalSave");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }

//        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
//        {
//            if (savesArray.Any())
//            {
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }
//        }

//        // метод используй как ивент
//        public void DropOldSaves()
//        {
//            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
//            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
//            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

//            if (isCharacterSelected && isSceneChanged)
//            {
//                PrepareOldSaves();
//            }
//        }

//        private void PrepareOldSaves()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }

//            if (savesArray!= null)
//            {
//                int counter = savesArray.Count;
//                // делаю unset
//                UnsetPlayerPrefsListIndexes(savesArray);

//                for (int i = 0; i < counter; i++)
//                {
//                    Debug.Log(savesArray[savesArray.Count - 1]);

//                    //----------------
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
//                    //----------------
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                }
//                GetComponent<SavingSystem>().Delete("portalSave");
//            }
//        }
//    }
//}











//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;
//using UnityEngine.SceneManagement;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        private string saveFileName;
//        public List<string> savesArray;


//        [SerializeField] float fadeInTime = 0.2f;


//        private void Update()
//        {
//            //=======================================================
//            // ====================ОСТОРОЖНО!!!
//            //=======================================================
//            if (PlayerPrefs.GetString("isSceneLoaded") == "true")
//            {
//                Load(false, null);
//                PlayerPrefs.SetString("isSceneLoaded", "false");
//            }
//            //========================================================

//            if (Input.GetKeyDown(KeyCode.L))
//            {
//                Load(false, null);
//            }
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                Save(false);
//            }
//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                Delete(null);
//            }
//        }


//        IEnumerator Start()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }
//            else
//            {
//                savesArray = new List<string>();

//                //// во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
//                //for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                //{
//                //    PlayerPrefs.DeleteKey(i.ToString());
//                //}
//                //PlayerPrefs.DeleteKey("currentArrayCount");
//            }

//            Fader fader = FindObjectOfType<Fader>();
//            //
//            if (fader != null)
//            {
//                fader.FadeOutImmediate();
//                if (savesArray.Any())
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); // после первого запуска загрузится последний сейв
//                }
//                else
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene("save");    // или это если нету сохранений
//                }
//                yield return fader.FadeIn(fadeInTime);
//            }
//        }



//        public void Save(bool isFromPortal)
//        {
//            if (!isFromPortal)
//            {
//                saveFileName = string.Format(@"{0}", Guid.NewGuid());
//                //------------------------------------
//                // делаем unset для старого списка
//                UnsetPlayerPrefsListIndexes(savesArray);
//                //------------------------------------

//                savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

//                //------------------------------------
//                // сохраняем в PlayerPrefs все индексы нового списка
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                }
//                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                //------------------------------------

//                GetComponent<SavingSystem>().Save(saveFileName);
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Save("portalSave");
//            }
//        }



//        public void Load(bool isFromPortal, string saveName)
//        {
//            if (!isFromPortal)
//            {
//                if (savesArray.Any())
//                {
//                    if (saveName == null)
//                    {
//                        GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для загрузки("L") - загрузи последнее
//                    }
//                    else
//                    {
//                        GetComponent<SavingSystem>().Load(saveName);
//                    }

//                    //===============================================================================
//                    // возможно в старте придётся делать похожее если не будет нормально отрабатывать
//                    // имя файла savesArray[savesArray.Count - 1]
//                    //===============================================================================
//                    GameObject PO = GameObject.Find("Player");
//                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;
//                    // если текущая сцена != сцене из сохранения
//                    if (SceneManager.GetActiveScene().buildIndex != requiredScene)
//                    {
//                        //ОСТОРОЖНО!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                        PlayerPrefs.SetString("isSceneLoaded", "true");
//                        SceneManager.LoadScene(requiredScene);
//                    }
//                    //===============================================================================
//                }
//                else
//                {
//                    GetComponent<SavingSystem>().Load("save");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("portalSave");
//            }
//        }


//        public void Delete(string saveName)
//        {
//            if (savesArray.Any())
//            {
//                if (saveName == null)
//                {
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для удаления - удали последнее
//                }
//                else
//                {
//                    GetComponent<SavingSystem>().Delete(saveName);
//                }

//                savesArray.RemoveAt(savesArray.Count - 1);
//                // делаю unset удалённого элемента
//                PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
//                if (savesArray.Count == 0)
//                {
//                    PlayerPrefs.DeleteKey("currentArrayCount");
//                    GetComponent<SavingSystem>().Delete("portalSave");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }

//        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
//        {
//            if (savesArray.Any())
//            {
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }
//        }

//        // метод используй как ивент
//        public void DropOldSaves()
//        {
//            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
//            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
//            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

//            if (isCharacterSelected && isSceneChanged)
//            {
//                PrepareOldSaves();
//            }
//        }

//        private void PrepareOldSaves()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }

//            if (savesArray != null)
//            {
//                int counter = savesArray.Count;
//                // делаю unset
//                UnsetPlayerPrefsListIndexes(savesArray);

//                for (int i = 0; i < counter; i++)
//                {
//                    Debug.Log(savesArray[savesArray.Count - 1]);

//                    //----------------
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
//                    //----------------
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                }
//                GetComponent<SavingSystem>().Delete("portalSave");
//            }
//        }
//    }
//}











//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Saving;
//using System;
//using System.Linq;
//using UnityEngine.SceneManagement;

//namespace RPG.SceneManagement
//{
//    public class SavingWrapper : MonoBehaviour
//    {
//        private string saveFileName;
//        public List<string> savesArray;


//        [SerializeField] float fadeInTime = 0.2f;



//        private void Update()
//        {
//            //=======================================================
//            // ====================ОСТОРОЖНО!!!
//            //=======================================================


//            if (PlayerPrefs.GetString("isSceneLoaded") == "true")
//            {
//                PlayerPrefs.SetString("isSceneLoaded", "false");
//                if (PlayerPrefs.GetString("isFromMenu") == "true")
//                {
//                    PlayerPrefs.SetString("isFromMenu", "false");
//                    print("PlayerPrefsSaveName = " + PlayerPrefs.GetString("saveName"));
//                    Load(false, PlayerPrefs.GetString("saveName"));
//                }
//                else
//                {
//                    Load(false, null);
//                }
//                //PlayerPrefs.SetString("saveName", null);
//            }
//            //if (PlayerPrefs.GetString("csElseEntered") == "true")
//            //{
//            //    // баг когда лоад данных выполнился, а сцена как будто не перезагрузилась(мёртвые враги не встают даже на тех сохранениях где у них было хп)
//            //    PlayerPrefs.SetString("csElseEntered", "false");
//            //    GetComponent<SavingSystem>().Load(PlayerPrefs.GetString("csElseEnteredSaveName"));                  // ПОЧЕМУ НЕ ОТРАБАТЫВАЕТ??????!!!!!!!!!!
//            //    Debug.Log("ЫЫЫЫЫЫЫЫЫЫЫЫЫЫ!!!!!!!!!!!!!!!!!!!"+ PlayerPrefs.GetString("csElseEnteredSaveName"));
//            //}
//            //========================================================

//            if (Input.GetKeyDown(KeyCode.L))
//            {
//                Load(false, null);
//            }
//            if (Input.GetKeyDown(KeyCode.S))
//            {
//                Save(false);
//            }
//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                Delete(null, -1);
//            }
//        }


//        IEnumerator Start()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }
//            else
//            {
//                savesArray = new List<string>();

//                //// во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
//                //for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                //{
//                //    PlayerPrefs.DeleteKey(i.ToString());
//                //}
//                //PlayerPrefs.DeleteKey("currentArrayCount");
//            }

//            Fader fader = FindObjectOfType<Fader>();
//            //
//            if (fader != null)
//            {
//                fader.FadeOutImmediate();
//                if (savesArray.Any())
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); // после первого запуска загрузится последний сейв
//                }
//                else
//                {
//                    yield return GetComponent<SavingSystem>().LoadLastScene("save");    // или это если нету сохранений
//                }
//                yield return fader.FadeIn(fadeInTime);
//            }
//        }



//        public void Save(bool isFromPortal)
//        {
//            if (!isFromPortal)
//            {
//                saveFileName = string.Format(@"{0}", Guid.NewGuid());
//                //------------------------------------
//                // делаем unset для старого списка
//                UnsetPlayerPrefsListIndexes(savesArray);
//                //------------------------------------

//                savesArray.Add(saveFileName);       // доб новый элем в список файлов сохранения

//                //------------------------------------
//                // сохраняем в PlayerPrefs все индексы нового списка
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.SetString(i.ToString(), savesArray[i]);
//                }
//                PlayerPrefs.SetInt("currentArrayCount", savesArray.Count);
//                //------------------------------------

//                GetComponent<SavingSystem>().Save(saveFileName);
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Save("portalSave");
//            }
//        }



//        public void Load(bool isFromPortal, string saveName)
//        {
//            Debug.Log("Load Enter");
//            PlayerPrefs.SetString("saveName", "null");
//            if (!isFromPortal)
//            {
//                if (savesArray.Any())
//                {
//                    if (saveName == null)
//                    {
//                        GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для загрузки("L") - загрузи последнее
//                    }
//                    else
//                    {
//                        //PlayerPrefs.SetString("csElseEntered", "true");
//                        //PlayerPrefs.SetString("csElseEnteredSaveName", saveName);
//                        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

//                        GetComponent<SavingSystem>().Load(saveName);
//                    }

//                    //===============================================================================
//                    // возможно в старте придётся делать похожее если не будет нормально отрабатывать
//                    // имя файла savesArray[savesArray.Count - 1]
//                    //===============================================================================
//                    GameObject PO = GameObject.Find("Player");
//                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;
//                    // если текущая сцена != сцене из сохранения
//                    if (SceneManager.GetActiveScene().buildIndex != requiredScene)
//                    {
//                        //ОСТОРОЖНО!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                        PlayerPrefs.SetString("isSceneLoaded", "true");
//                        if (saveName != null)
//                        {
//                            PlayerPrefs.SetString("isFromMenu", "true");

//                            PlayerPrefs.SetString("saveName", saveName);
//                        }
//                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                        SceneManager.LoadScene(requiredScene);
//                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                        //если грузишься через UI
//                        // в старте отрабатывает Load(false, null);     и грузит последнее сохранение
//                        // а должно отрабатать Load(false, saveName);
//                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                    }
//                    //===============================================================================
//                }
//                else
//                {
//                    GetComponent<SavingSystem>().Load("save");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Load("portalSave");
//            }
//        }


//        public void Delete(string saveName, int saveIndex)
//        {
//            if (savesArray.Any())
//            {
//                if (saveName == null)
//                {
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);  // если была нажата кнопка для удаления - удали последнее
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                    // делаю unset удалённого элемента
//                    PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
//                }
//                else
//                {
//                    GetComponent<SavingSystem>().Delete(saveName);

//                    // нужно сместить массив PlayerPrefs'ов и потом последний удалится ниже(PlayerPrefs.DeleteKey)
//                    // (0,1,2) => (0,2,2)
//                    if (saveIndex != 0)
//                    {
//                        for (int i = savesArray.Count - 1; i >= saveIndex; i--)
//                        {
//                            PlayerPrefs.SetString(i.ToString(), savesArray[i - 1]);
//                            //savesArray[i] = savesArray[i - 1];
//                        }
//                    }
//                    savesArray.RemoveAt(saveIndex);                                     ///////////////// удалил из списка
//                    // делаю unset удалённого элемента
//                    PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());           ///////////////// удалил из PlayerPrefs
//                }


//                if (savesArray.Count == 0)
//                {
//                    PlayerPrefs.DeleteKey("currentArrayCount");
//                    GetComponent<SavingSystem>().Delete("portalSave");
//                }
//            }
//            else
//            {
//                GetComponent<SavingSystem>().Delete("save");
//            }
//        }

//        private void UnsetPlayerPrefsListIndexes(List<string> savesArray)
//        {
//            if (savesArray.Any())
//            {
//                for (int i = 0; i < savesArray.Count; i++)
//                {
//                    PlayerPrefs.DeleteKey(i.ToString());
//                }
//                PlayerPrefs.DeleteKey("currentArrayCount");
//            }
//        }

//        // метод используй как ивент
//        public void DropOldSaves()
//        {
//            // смена ГГ = начало новой игры и = удаление всех прошлых сохранений
//            bool isCharacterSelected = SelectedCharacter.CheckNewGameStart()[0];
//            bool isSceneChanged = SelectedCharacter.CheckNewGameStart()[1];

//            if (isCharacterSelected && isSceneChanged)
//            {
//                PrepareOldSaves();
//            }
//        }

//        private void PrepareOldSaves()
//        {
//            int arrCount = PlayerPrefs.GetInt("currentArrayCount");
//            if (arrCount != 0)
//            {
//                savesArray = new List<string>();
//                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
//                {
//                    savesArray.Add(PlayerPrefs.GetString(i.ToString()));
//                }
//            }

//            if (savesArray != null)
//            {
//                int counter = savesArray.Count;
//                // делаю unset
//                UnsetPlayerPrefsListIndexes(savesArray);

//                for (int i = 0; i < counter; i++)
//                {
//                    Debug.Log(savesArray[savesArray.Count - 1]);

//                    //----------------
//                    GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);
//                    //----------------
//                    savesArray.RemoveAt(savesArray.Count - 1);
//                }
//                GetComponent<SavingSystem>().Delete("portalSave");
//            }
//        }
//    }
//}
#endregion