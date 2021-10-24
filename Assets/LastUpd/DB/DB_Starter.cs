using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_Starter : MonoBehaviour
{

    DB_Controller contr;
    private void Start()
    {
        contr = FindObjectOfType<DB_Controller>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && contr.imReady == true)
        {
            contr.imReady = false;
            StartCoroutine(contr.Do());
        }
    }
}
