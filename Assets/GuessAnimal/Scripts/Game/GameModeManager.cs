using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    private static GameModeManager _instance;
    public static GameModeManager instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] GameMode gameMode;
    public void SelectGameMode(string mode)
    {
        if (mode == "Name")
        {
            gameMode = GameMode.Image;
        }
        else
        {
            gameMode = GameMode.Audio;
        }
    }

    public GameMode ActiveGameMode()
    {
        return gameMode;
    }

}
