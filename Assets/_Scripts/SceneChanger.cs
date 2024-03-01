using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Reference to the scene you want to change to
    public string sceneToLoad;

    // Function to change the scene
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
