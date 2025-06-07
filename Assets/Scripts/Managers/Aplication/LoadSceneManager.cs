using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : Manager
{
    public override void Deinitialize()
    {
    }

    public override void Initialize()
    {
    }

    public override void UpdateManager()
    {
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
