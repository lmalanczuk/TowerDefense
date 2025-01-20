using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Funkcja wywo³ywana przez przycisk "START"
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Funkcja wywo³ywana przez przycisk "EXIT"
    public void ExitGame()
    {
        // Zamyka aplikacjê (w edytorze Unity nie zadzia³a bez dodatkowego polecenia)
        Application.Quit();

        // Opcjonalne wyjœcie z trybu Play w edytorze Unity (tylko w edytorze)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
