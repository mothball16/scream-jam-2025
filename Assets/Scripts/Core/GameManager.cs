using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int CurrentDay { get; set; }
    

    public override void Init()
    {

    }

    public void LoadLevel(int day)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        EventBus.Publish(new LoadLevelEvent(day));
    }
}