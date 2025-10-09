using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _initScene = "Gameplay";
    private void Awake()
    {
        SceneManager.LoadScene(_initScene, LoadSceneMode.Additive);
    }
}
