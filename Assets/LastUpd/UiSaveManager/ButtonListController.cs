using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonListController : MonoBehaviour
{
    // ОБЯЗАТЕЛЬНО ПЕРЕДЕЛАЙ SavesMenu СЦЕНУ НА ESC МЕНЮШКУ!!!
    // изначально пусть игра грузит либо 1 сцену, либо последнее сохранение(для этого даже ничего не нужно менять :) )
    // нажатие на Esc - будет вызывать SavesMenu. Переделаешь его чтоб это была не отдельная сцена, а вылазила менюшка в игре


    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] public int changeCharacterSceneIndex = 0;
    private List<string> savesList;
    private List<GameObject> buttons;
    private SavingWrapper savingWrapper;
    int savingIndex = -1;

    private void Start()
    {
        buttons = new List<GameObject>();
        savingWrapper = FindObjectOfType<SavingWrapper>();
        // не запустится ли этот Start раньше чем отработает Start SavingWrapper'а???
        GenerateList();
    }
    public void GenerateList()
    {
        int arrCount = PlayerPrefs.GetInt("currentArrayCount");
        if (arrCount != 0)
        {
            if (buttons.Count > 0)
            {
                foreach (GameObject button in buttons)
                {
                    Destroy(button.gameObject);
                }
                buttons.Clear();
            }

            savesList = savingWrapper.savesArray;
            for (int i = 0; i < savesList.Count; i++)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);

                button.GetComponent<ButtonListWorker>().SetText(savesList[i], i);
                button.transform.SetParent(buttonTemplate.transform.parent, false);
                buttons.Add(button);
            }
        }
        else
        {
            Debug.Log("data is Empty");
            foreach (GameObject button in buttons)
            {
                Destroy(button.gameObject);
            }
            buttons.Clear();

        }
    }


    private string saveName = null;
    public void ButtonClicked(string myTextString, int index)
    {
        Debug.Log(myTextString);
        saveName = myTextString;
        savingIndex = index;
    }

    public void SaveGame()  // кнопка сбоку
    {
        savingWrapper.Save(false);  // просто новое сохранение, имя файла сгенерируется автоматически
        // обновление UI списка
        GenerateList();
    }

    public void LoadSave() // зелёная кнопка сбоку
    {
        if (saveName != null)
        {
            savingWrapper.Load(false, saveName, true);
            //saveName = null; // не пишу потому, что если выделено сохранение то пусть меняют либо грузят то, что уже выделили
        }
    }
    public void DeleteSave() // красная кнопка сбоку
    {
        if (saveName != null)
        {
            //savingWrapper.Delete(saveName, savingIndex);
            savingWrapper.Delete(saveName, savingIndex);
            saveName = null;    // чтоб не пытались повторно удалить то чего уже нету
        }
        GenerateList();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToChangeCharacter()
    {
        SceneManager.LoadScene(changeCharacterSceneIndex);
    }
}