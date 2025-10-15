using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string _scene;
    public void SwitchScene()
    {
        SceneManager.LoadScene(_scene);

    }
}
