using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    None,
    Easy,
    Medium,
    Hard,
    Extreme,
}
public class MenuController : MonoBehaviour
{
    public GameObject game;

    public Button start;
    public Button select_mode;
    public Button easy;
    public Button medium;
    public Button hard;
    public Button extreme;
    public Button continue_button;
    public Button exit;
    public Button high_scores;
    public Button learn;
    public GameObject new_game_warning;
    public GameObject end_game_warning;
    public Button affirm_end_game;
    public Button affirm_new_game;
    public Button play;
    public GameObject scores;

    public FileHandler file_handler;
    public GameData data;

    public static int NumEasyGuess = 20;
    public static int NumMedGuess = 15;
    public static int NumHardGuess = 10;
    public static int NumExtremeGuess = 7;
    public static int MaxGuessesAllowed = 20;
    public static int NumCountries = 166;

    public static float EasyTime = 120f;
    public static float MedTime = 60f;
    public static float HardTime = 30f;
    public static float ExtremeTime = 15f;

    public static int EastMult = 10;
    public static int MedMult = 25;
    public static int HardMult = 50;
    public static int ExtremeMult = 100;

    public static string Game_File_Name = "data.game";

    private bool hs_clicked = true;
    public GameMode mode;

    private void Start()
    {
        start.onClick.AddListener(TaskOnStartClick);
        easy.onClick.AddListener(TaskOnEasyClick);
        medium.onClick.AddListener(TaskOnMediumClick);
        hard.onClick.AddListener(TaskOnHardClick);
        extreme.onClick.AddListener(TaskOnExtremeClick);
        high_scores.onClick.AddListener(TaskOnHighScoreClick);
        continue_button.onClick.AddListener(TaskOnContinueClick);
        exit.onClick.AddListener(TaskOnExitClick);
        learn.onClick.AddListener(TaskOnLearnClick);
        affirm_new_game.onClick.AddListener(OnAffirmNewGameClick);
        play.onClick.AddListener(TaskOnPlayClick);
        select_mode.onClick.AddListener(TaskOnCloseClick);
        affirm_end_game.onClick.AddListener(TaskOnAffirmEndGameClick);

        file_handler = new FileHandler(Application.persistentDataPath, Game_File_Name);
        data = file_handler.Load();

        if(data == null)
        {
            data = new GameData();
        }
    }

    private void Update()
    {
        // Just to make sure player cannot 'pause' by going to the menu
        if(game.GetComponent<CountryFinder>().game_timer > 0.0f)
        {
            game.GetComponent<CountryFinder>().game_timer -= Time.deltaTime;
        }
    }
    void TaskOnStartClick()
    {
        start.gameObject.SetActive(false);
        switchSelectMode(true);
        continue_button.gameObject.SetActive(false);
        learn.gameObject.SetActive(false);
    }
    void OnAffirmNewGameClick()
    {
        StartNewGame();
    }
    void TaskOnEasyClick()
    {
        mode = GameMode.Easy;

        if (data.game_in_progress)
        {
            new_game_warning.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    void TaskOnMediumClick()
    {
        mode = GameMode.Medium;

        if (data.game_in_progress)
        {
            new_game_warning.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    void TaskOnHardClick()
    {
        mode = GameMode.Hard;

        if (data.game_in_progress)
        {
            new_game_warning.SetActive(true);
        }
        else
        { 
            StartNewGame();
        }
    }

    void TaskOnExtremeClick()
    {
        mode = GameMode.Extreme;

        if (data.game_in_progress)
        {
            new_game_warning.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    void TaskOnPlayClick()
    {
        high_scores.gameObject.transform.position = new Vector3(high_scores.gameObject.transform.position.x, Screen.height / 2f);
        scores.SetActive(false);
        start.gameObject.SetActive(true);
        continue_button.gameObject.SetActive(true);
        learn.gameObject.SetActive(true);

        play.gameObject.SetActive(false);
    }
    void TaskOnHighScoreClick()
    {
        start.gameObject.SetActive(false);
        continue_button.gameObject.SetActive(false);
        learn.gameObject.SetActive(false);

        if (hs_clicked)
        {
            high_scores.gameObject.transform.position = new Vector3(high_scores.gameObject.transform.position.x, Screen.height * 0.8f);
            scores.gameObject.SetActive(true);
        }
        else
        {
            high_scores.gameObject.transform.position = new Vector3(high_scores.gameObject.transform.position.x, Screen.height / 2f);
            scores.gameObject.SetActive(false);
        }

        hs_clicked = !hs_clicked;
        switchSelectMode(false);
    }

    void TaskOnContinueClick()
    {
        gameObject.SetActive(false);
        game.SetActive(true);
        game.GetComponent<CountryFinder>().ContinueGame();

        TurnOffExtraMenuItems();
    }

    void TurnOffExtraMenuItems()
    {
        play.gameObject.SetActive(true);
        start.gameObject.SetActive(false);
        continue_button.gameObject.SetActive(false);
        learn.gameObject.SetActive(false);
        new_game_warning.gameObject.SetActive(false);
        end_game_warning.gameObject.SetActive(false);
        switchSelectMode(false);
    }

    void TaskOnExitClick()
    {
        Application.Quit();
    }

    void TaskOnLearnClick()
    {
        if (data.game_in_progress)
        {
            end_game_warning.SetActive(true);
        }
        else
        {
            StartLearnGame();
        }
    }

    void TaskOnAffirmEndGameClick()
    {
        StartLearnGame();
    }

    void StartLearnGame()
    {
        game.SetActive(true);
        gameObject.SetActive(false);
        TurnOffExtraMenuItems();
        game.GetComponent<GameUI>().NewHighScore(false);
        game.GetComponent<CountryFinder>().StartLearnGame();
    }

    void TaskOnCloseClick()
    { 
        start.gameObject.SetActive(true);
        continue_button.gameObject.SetActive(true);
        learn.gameObject.SetActive(true);
        switchSelectMode(false);
    }

    void switchSelectMode(bool active)
    {
        select_mode.gameObject.SetActive(active);
        easy.gameObject.SetActive(active);
        medium.gameObject.SetActive(active);
        hard.gameObject.SetActive(active);
        extreme.gameObject.SetActive(active);
    }

    private void StartNewGame()
    {
        data = new GameData();
        data.mode_data = mode;
        gameObject.SetActive(false);
        game.SetActive(true);
        game.GetComponent<CountryFinder>().StartNewGame();
        TurnOffExtraMenuItems();
    }
}
