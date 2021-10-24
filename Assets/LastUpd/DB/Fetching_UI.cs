using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fetching_UI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GetComponent<Animator>().SetTrigger("startFetching");
        }
    }
}
