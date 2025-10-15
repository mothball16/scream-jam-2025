using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }
    }

    public int currentDay { get; set; } = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(currentDay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLevel(int day)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        EventBus.Publish(new LoadLevelEvent(day));
    }
}