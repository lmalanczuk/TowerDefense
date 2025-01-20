using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Funkcja wywo�ywana przez przycisk "START"
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Funkcja wywo�ywana przez przycisk "EXIT"
    public void ExitGame()
    {
        // Zamyka aplikacj� (w edytorze Unity nie zadzia�a bez dodatkowego polecenia)
        Application.Quit();

        // Opcjonalne wyj�cie z trybu Play w edytorze Unity (tylko w edytorze)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
