using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListWorker : MonoBehaviour
{
    [SerializeField] private Text myText;
    [SerializeField] private ButtonListController buttonController;
    private string myTextString;
    private int saveInd = -1;


    public void SetText(string textString, int index)
    {
        myTextString = textString;
        myText.text = $"Save {index+1}";
        saveInd = index;
    }


    static GameObject prevSaveButton = null;
    static Text prevText = null;

    public void OnClick()
    {
        buttonController.ButtonClicked(myTextString, saveInd);


        if (prevSaveButton != null && prevText != null)
        {
            prevSaveButton.GetComponent<Image>().color = Color.white;
            prevText.color = Color.black;
        }

        prevSaveButton = gameObject;
        prevText = myText;
        
        prevSaveButton.GetComponent<Image>().color = Color.black;
        prevText.color = Color.white;
    }
}
