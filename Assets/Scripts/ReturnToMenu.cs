using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for retuning the user the menu
/// </summary>
public class ReturnToMenu : MonoBehaviour
{

    /// <summary>
    /// Returns to main menu.
    /// </summary>
    public void ReturnToMainMenu()
    {

        SceneManager.LoadScene(0);
    }		
}
