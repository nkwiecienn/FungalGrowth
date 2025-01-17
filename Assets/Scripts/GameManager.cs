using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ClearAndReset()
    {
        SceneManager.LoadScene(0);
    }
}
