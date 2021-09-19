using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;

public class SaveSceneIndex : MonoBehaviour, ISaveable
{
    public int requiredLoadSceneIndex;
    private void Start()
    {
        requiredLoadSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    public object CaptureState()
    {
        return requiredLoadSceneIndex;
    }

    public void RestoreState(object state)
    {
        requiredLoadSceneIndex = (int)state;
    }
}
