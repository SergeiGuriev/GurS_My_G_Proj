using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedCharacter : MonoBehaviour
{
    private int characterIndex = 0;
    private int currenCharacter;

    public GameObject[] allCharacters;
    public GameObject arrowToLeft;
    public GameObject arrowToRight;

    public GameObject buttonSelectCharacter;
    public GameObject textSelectCharacter;


    public static bool isCharacterSelected;
    public static bool isSceneChanged;

    void Start()
    {
        isCharacterSelected = false;
        isSceneChanged = false;

        if (PlayerPrefs.HasKey("CurrentCharacter"))
        {
            characterIndex = PlayerPrefs.GetInt("CurrentCharacter");
            currenCharacter = PlayerPrefs.GetInt("CurrentCharacter");
        }
        else
        {
            PlayerPrefs.SetInt("CurrentCharacter", characterIndex);
        }
        allCharacters[characterIndex].SetActive(true);

        buttonSelectCharacter.SetActive(false);
        textSelectCharacter.SetActive(true);

        if (characterIndex > 0)
        {
            arrowToLeft.SetActive(true);
        }
        if (characterIndex == allCharacters.Length-1)
        {
            arrowToRight.SetActive(false);
        }
    }
    public void ArrowRight()
    {
        if (characterIndex < allCharacters.Length)
        {
            if (characterIndex == 0)
            {
                arrowToLeft.SetActive(true);
            }
            allCharacters[characterIndex].SetActive(false);
            characterIndex++;
            allCharacters[characterIndex].SetActive(true);

            if (currenCharacter == characterIndex)
            {
                buttonSelectCharacter.SetActive(false);
                textSelectCharacter.SetActive(true);
            }
            else
            {
                buttonSelectCharacter.SetActive(true);
                textSelectCharacter.SetActive(false);
            }
            if (characterIndex == allCharacters.Length - 1)
            {
                arrowToRight.SetActive(false);
            }
        }
    }

    public void ArrowLeft()
    {
        if (characterIndex < allCharacters.Length)
        {
            allCharacters[characterIndex].SetActive(false);
            characterIndex--;
            allCharacters[characterIndex].SetActive(true);
            arrowToRight.SetActive(true);

            if (currenCharacter == characterIndex)
            {
                buttonSelectCharacter.SetActive(false);
                textSelectCharacter.SetActive(true);
            }
            else
            {
                buttonSelectCharacter.SetActive(true);
                textSelectCharacter.SetActive(false);
            }
            if (characterIndex == 0)
            {
                arrowToLeft.SetActive(false);
            }
        }
    }

    public void SelectCharacter()
    {
        isCharacterSelected = true;

        currenCharacter = characterIndex;
        PlayerPrefs.SetInt("CurrentCharacter", currenCharacter);
        buttonSelectCharacter.SetActive(false);
        textSelectCharacter.SetActive(true);
    }
    public void ChangeScene()
    {
        isSceneChanged = true;

        SceneManager.LoadScene(1);
    }

    public static bool[] CheckNewGameStart()
    {
        bool[] arr = new[]
            {
              isCharacterSelected,
              isSceneChanged
            };
        return arr;
    }
}
