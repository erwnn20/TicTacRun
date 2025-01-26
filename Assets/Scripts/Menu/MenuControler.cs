using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu controller to manage player interactions with the user interface.
/// Allows changing scenes and quitting the game.
/// </summary>
public class MenuControler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { }

    // Update is called once per frame
    void Update()
    { }
    
    /// <summary>
    /// Loads a new scene based on the specified name.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void ChangeScenePlay(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// Exits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    
    
    
    
    
    
    
}
