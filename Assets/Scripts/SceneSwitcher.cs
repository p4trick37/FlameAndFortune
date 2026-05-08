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
    [SerializeField] private string nextSceneName;   // Used in builds
    [SerializeField] private bool useSpaceToContinue = false;

    private bool currentlyInPanel;
    private PowerCord powerCord;

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Auto-fill the scene name whenever the SceneAsset changes
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
            powerCord.ExitWirePanel();
            LoadNextScene();
        }
    }

    // 🔹 Used by UI Button
    public void SwitchToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Used for spacebar progression
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

    public void GetPowerCord(PowerCord cord)
    {
        powerCord = cord;
    }
}
