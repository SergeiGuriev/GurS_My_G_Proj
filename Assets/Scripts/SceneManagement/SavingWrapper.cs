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
        List<string> savesArray;


        [SerializeField] float fadeInTime = 0.2f;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load(false);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save(false);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }


        // сохранение в портале отрабатывает 2 раза. 
        // и второй раз перезаписывает первую запись


            

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

                // во время написания кода могут вылазить ошибки, НО PlayerPrefs ключи всё равно могут успеть установиться! Поэтому нужно их удалить
                for (int i = 0; i < PlayerPrefs.GetInt("currentArrayCount"); i++)
                {
                    PlayerPrefs.DeleteKey(i.ToString());
                }
                PlayerPrefs.DeleteKey("currentArrayCount");
            }

            Fader fader = FindObjectOfType<Fader>();
            //
            if (fader != null)
            {
                fader.FadeOutImmediate();
                if (savesArray.Any())
                {
                    yield return GetComponent<SavingSystem>().LoadLastScene(savesArray[savesArray.Count - 1]); //нужно будет передавать имя файла сохранения
                }
                else
                {
                    yield return GetComponent<SavingSystem>().LoadLastScene("save");
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

        

        public void Load(bool isFromPortal)
        {
            if (!isFromPortal)
            {
                if (savesArray.Any())
                {
                    GetComponent<SavingSystem>().Load(savesArray[savesArray.Count - 1]);    // нужно будет передавать имя файла сохранения

                    //===============================================================================
                    GameObject PO = GameObject.Find("Player");
                    int requiredScene = PO.GetComponent<SaveSceneIndex>().requiredLoadSceneIndex;
                    // если текущая сцена != сцене из сохранения
                    if (SceneManager.GetActiveScene().buildIndex != requiredScene)
                    {
                        SceneManager.LoadScene(requiredScene);
                        // ПОСЛЕ ЭТОГО ЛОАДА НУЖНО КАК-ТО ЗАПУСТИТЬ ЕЩЁ 1 ЛОАД!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    }
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


        public void Delete()
        {
            if (savesArray.Any())
            {
                GetComponent<SavingSystem>().Delete(savesArray[savesArray.Count - 1]);      // нужно будет передавать имя файла сохранения
                savesArray.RemoveAt(savesArray.Count - 1);
                // делаю unset удалённого элемента
                PlayerPrefs.DeleteKey((savesArray.Count - 1).ToString());
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

        public void PrepareOldSaves()
        {
            if (savesArray.Any())
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
            }
        }
    }
}