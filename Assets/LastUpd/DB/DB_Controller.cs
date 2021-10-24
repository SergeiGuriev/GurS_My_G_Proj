using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DB_Controller : MonoBehaviour
{
    public string[] data;
    public bool imReady = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && imReady == true)
        {
            imReady = false;
            StartCoroutine(Do());
        }
    }

    public IEnumerator Do()
    {
        //delete
        yield return DeleteData();

        //insert    исп полиморфизм
        yield return InsertData();

        //select            
        //yield return StartCoroutine(SelectData());
        yield return imReady = true;
    }


    // селект нужно вызывать ТОЛЬКО после того как база полностью заполнится
    IEnumerator SelectData()
    {
        WWW states = new WWW("http://localhost/rpgdb/selectData.php");
        yield return states;
        string statesDataString = states.text;
        data = statesDataString.Split(';');

        for (int i = 0; i < data.Length - 1; i++)
        {
            print(
                        GetValueData(data[i], "scene_name:") +
                " | " + GetValueData(data[i], "character_class:") +
                " | " + GetValueData(data[i], "health:") +
                " | " + GetValueData(data[i], "experience_reward:") +
                " | " + GetValueData(data[i], "experience_to_lvl_up:") +
                " | " + GetValueData(data[i], "damage:") +
                " | " + GetValueData(data[i], "level:")
                );
        }
    }
    string GetValueData(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
        {
            value = value.Remove(value.IndexOf("|"));
        }
        return value;
    }


    public IEnumerator InsertData()
    {
        WWWForm form = new WWWForm();
        foreach (IDbProvider item in GetComponents<IDbProvider>())
        {
            form.AddField("addSceneName", item.GetStates()[0].ToString());
            form.AddField("addCharacterClass", item.GetStates()[1].ToString());
            form.AddField("addHealth", Convert.ToInt32(item.GetStates()[2]));
            form.AddField("addExperienceReward", Convert.ToString(item.GetStates()[3]));
            form.AddField("addExperienceToLvlUp", Convert.ToString(item.GetStates()[4]));
            form.AddField("addDamage", Convert.ToInt32(item.GetStates()[5]));
            form.AddField("addLevel", Convert.ToInt32(item.GetStates()[6]));
        }
        yield return new WWW("http://localhost/rpgdb/insertData.php", form);
    }

    public IEnumerator DeleteData()
    {
        yield return new WWW("http://localhost/rpgdb/deleteData.php");
    }
}





//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;

//public class DB_Controller : MonoBehaviour
//{
//    public string[] data;
//    bool imReady = true;
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Tab) && imReady == true)
//        {
//            imReady = false;
//            StartCoroutine(Do());
//        }
//    }


//    IEnumerator Do()
//    {
//        //delete
//        yield return DeleteData();

//        //insert    исп полиморфизм
//        yield return InsertData("test1", "test1", 1, 1, 1, 1);
//        yield return InsertData("test2", "test2", 2, 2, 2, 2);
//        yield return InsertData("test3", "test3", 3, 3, 3, 3);

//        //select            
//        yield return StartCoroutine(SelectData());
//        yield return imReady = true;
//    }

//    IEnumerator SelectData()
//    {
//        WWW states = new WWW("http://localhost/rpgdb/selectData.php");
//        yield return states;
//        string statesDataString = states.text;
//        data = statesDataString.Split(';');

//        for (int i = 0; i < data.Length - 1; i++)
//        {
//            print(
//                        GetValueData(data[i], "scene_name:") +
//                " | " + GetValueData(data[i], "character_class:") +
//                " | " + GetValueData(data[i], "health:") +
//                " | " + GetValueData(data[i], "experience_reward:") +
//                " | " + GetValueData(data[i], "experience_to_lvl_up:") +
//                " | " + GetValueData(data[i], "damage:")
//                );
//        }
//    }
//    string GetValueData(string data, string index)
//    {
//        string value = data.Substring(data.IndexOf(index) + index.Length);
//        if (value.Contains("|"))
//        {
//            value = value.Remove(value.IndexOf("|"));
//        }
//        return value;
//    }

//    public IEnumerator InsertData(string scene_name, string character_class, int health, int experience_reward, int experience_to_lvl_up, int damage)
//    {
//        WWWForm form = new WWWForm();
//        form.AddField("addSceneName", scene_name);
//        form.AddField("addCharacterClass", character_class);
//        form.AddField("addHealth", health);
//        form.AddField("addExperienceReward", experience_reward);
//        form.AddField("addExperienceToLvlUp", experience_to_lvl_up);
//        form.AddField("addDamage", damage);

//        yield return new WWW("http://localhost/rpgdb/insertData.php", form);
//    }

//    public IEnumerator DeleteData()
//    {
//        yield return new WWW("http://localhost/rpgdb/deleteData.php");
//    }
//}






















//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;

//public class DB_Controller : MonoBehaviour
//{
//    static Dictionary<GameObject, Dictionary<string, object>> fieldsTable = null;

//    public string[] data;
//    bool imReady = true;

//    private void Start()
//    {
//        fieldsTable = new Dictionary<GameObject, Dictionary<string, object>>();
//    }
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Tab) && imReady == true)
//        {
//            imReady = false;
//            StartCoroutine(Do());
//        }
//    }

//    public void SetImReady(bool imReady)
//    {
//        this.imReady = imReady;
//    }
//    public bool GetImReady()
//    {
//        return imReady;
//    }

//    public IEnumerator Do()
//    {
//        //delete
//        yield return DeleteData();

//        //insert    исп полиморфизм
//        yield return InsertData();

//        //select            
//        yield return StartCoroutine(SelectData());
//        yield return imReady = true;
//    }

//    IEnumerator SelectData()
//    {
//        WWW states = new WWW("http://localhost/rpgdb/selectData.php");
//        yield return states;
//        string statesDataString = states.text;
//        data = statesDataString.Split(';');

//        for (int i = 0; i < data.Length - 1; i++)
//        {
//            print(
//                        GetValueData(data[i], "scene_name:") +
//                " | " + GetValueData(data[i], "character_class:") +
//                " | " + GetValueData(data[i], "health:") +
//                " | " + GetValueData(data[i], "experience_reward:") +
//                " | " + GetValueData(data[i], "experience_to_lvl_up:") +
//                " | " + GetValueData(data[i], "damage:")
//                );
//        }
//    }
//    string GetValueData(string data, string index)
//    {
//        string value = data.Substring(data.IndexOf(index) + index.Length);
//        if (value.Contains("|"))
//        {
//            value = value.Remove(value.IndexOf("|"));
//        }
//        return value;
//    }



//    public IEnumerator InsertData()
//    {
//        WWWForm form = new WWWForm();
//        var c = GetComponents<IDbProvider>();
//        foreach (IDbProvider item in GetComponents<IDbProvider>())
//        {
//            form.AddField("addSceneName", item.GetStates()[0].ToString());
//            form.AddField("addCharacterClass", item.GetStates()[1].ToString());
//            form.AddField("addHealth", Convert.ToInt32(item.GetStates()[2]));
//            form.AddField("addExperienceReward", Convert.ToInt32(item.GetStates()[3]));
//            form.AddField("addExperienceToLvlUp", Convert.ToInt32(item.GetStates()[4]));
//            form.AddField("addDamage", Convert.ToInt32(item.GetStates()[5]));
//        }

//        yield return new WWW("http://localhost/rpgdb/insertData.php", form);
//    }

//    public IEnumerator DeleteData()
//    {
//        yield return new WWW("http://localhost/rpgdb/deleteData.php");
//    }
//}