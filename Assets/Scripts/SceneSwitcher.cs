using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSwitcher : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Next Scene (Drag in Inspector)")]
    [SerializeField] private SceneAsset nextScene;
#endif

    [Header("Settings")]
    [SerializeField] private bool useSpaceToContinue = false;

    private string nextSceneName;

    private void Awake()
    {
#if UNITY_EDITOR
        if (nextScene != null)
        {
            nextSceneName = nextScene.name;
        }
#endif
    }

    private void Update()
    {
        if (useSpaceToContinue && Input.GetKeyDown(KeyCode.Space))
        {
            Player.instance.ClearInventory();
            Player.instance.ResetInventory();
            LoadNextScene();
        }
    }

    // 🔹 Used by UI Button (your current setup)
    public void SwitchToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Used for tutorial progression (Space key)
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene assigned.");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}