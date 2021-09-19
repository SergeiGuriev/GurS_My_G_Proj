using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private int characterIndex;
    public GameObject[] AllCharacters;
    //public GameObject tempCamera;

    private void Awake()
    {
        characterIndex = PlayerPrefs.GetInt("CurrentCharacter");
        AllCharacters[characterIndex].SetActive(true);
        //tempCamera.SetActive(false);
    }
}
