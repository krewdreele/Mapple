using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState
{
    play,
    won,
    lost,
    learn
}
public class CountryFinder : MonoBehaviour
{
    public GameState state;
    public GameObject menu;

    private GameUI ui_ctrl;
    public MenuController menu_ctrl;
    private CamController cam_ctrl;
    private FileHandler file_ctrl;

    public Country goal_country;
    public List<string> guessed_countries;
    public List<Country> countryList = new List<Country>();

    public List<Color> color_list;
    private Color[] color_scale;
    private int num_colors = 15;

    private string text;
    public int num_guesses;
    public int guesses_allowed;
    public int streak;
    public bool new_hs = false;
    public bool hs_saved = false;

    public float game_timer;
    public int score;

    private float tap_timer = 0.0f;

    private void Awake()
    {
        file_ctrl = new FileHandler(Application.persistentDataPath, MenuController.Game_File_Name);

        //Add each country to list
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GameController"))
        {
            //Check if it has a nickname
            string nickname = "";
            if (go.GetComponent<Nickname>() != null)
            {
                nickname = go.GetComponent<Nickname>().nickname;
            }
            countryList.Add(new Country(go, go.name, nickname));
        }

        //Find country's flag
        foreach (Country c in countryList)
        {
            foreach (GameObject f in GameObject.FindGameObjectsWithTag("Flag"))
            {
                if (f.name.ToLower() == c.name.ToLower() || f.name.ToLower() == c.secondname.ToLower())
                {
                    f.SetActive(false);
                    c.flag = f;
                }
            }
        }

        //Load all of the country info
        file_ctrl.LoadCountryInfo("CO2InfoSorted", countryList);
        file_ctrl.LoadCountryInfo("CrimeInfoSorted", countryList);
        file_ctrl.LoadCountryInfo("EducationInfoSorted", countryList);
        file_ctrl.LoadCountryInfo("GDPInfoSorted", countryList);
        file_ctrl.LoadCountryInfo("LaborSorted", countryList);
        file_ctrl.LoadCountryInfo("PopulationDensitySorted", countryList);
        file_ctrl.LoadCountryInfo("PopulationInfoSorted", countryList);
        file_ctrl.LoadCountryInfo("LandInfoSorted", countryList);
        file_ctrl.LoadCountryInfo("InternetInfoSorted", countryList);

        //Get components
        menu_ctrl = menu.GetComponent<MenuController>();
        cam_ctrl = Camera.main.GetComponent<CamController>();
        ui_ctrl = gameObject.GetComponent<GameUI>();

        //Initialize variables
        guessed_countries = new List<string>();
        color_list = new List<Color>();

        GenerateColorScale();
    }

    public void StartLearnGame()
    {
        // Set UI and Camera
        ui_ctrl.LearnMode();
        cam_ctrl.ResetCam();

        //Reset data
        streak = 0;
        score = 0;
        NewRound();

        //Set game state
        state = GameState.learn;

        //Clear the goal country
        goal_country = null;
    }
    public void StartNewGame()
    {
        //Reset data
        streak = 0;
        score = 0;
        new_hs = false;
        NewRound();

        //Set UI
        ui_ctrl.GameMode();
        ui_ctrl.NewHighScore(false);

        //We have a game in progress
        menu_ctrl.data.game_in_progress = true;
    }

    public void ContinueGame()
    {
        //Set variables to loaded data
        num_guesses = menu_ctrl.data.num_guesses;
        streak = menu_ctrl.data.streak;
        score = menu_ctrl.data.score;
        SetGuessesAllowed();

        guessed_countries.Clear();
        color_list.Clear();
        
        switch (menu_ctrl.data.game_state)
        {
            case 0:
                state = GameState.play;
                break;

            case 1:
                state = GameState.won;
                break;

            case 2:
                state = GameState.lost;
                break;
        }

        foreach (string name in menu_ctrl.data.guessed_coutries)
        {
            if (name != "")
            {
                guessed_countries.Add(name.ToLower());
            }
        }

        foreach (Color c in menu_ctrl.data.country_colors)
        {
            if (c != Color.clear)
            {
                color_list.Add(c);
            }
        }

        //If there is no previous data, pick a random country
        if (menu_ctrl.data.goal_country == null)
        {
            goal_country = GetRandomCountry();
        }
        else
        {
            foreach(Country c in countryList)
            {
                if(c.name.ToLower() == menu_ctrl.data.goal_country.name.ToLower())
                {
                    goal_country = c;
                    break;
                }
            }
        }

        //Reset all country colors to white
        foreach (Country c in countryList)
        {
            c.go.GetComponent<SpriteRenderer>().color = Color.white;
        }

        //Change countries' color from loaded data
        int color_count = 0;
        if (guessed_countries.Count > 0)
        {
            foreach (string name in guessed_countries)
            {
                foreach (Country c in countryList)
                {
                    if (name == c.name.ToLower())
                    {
                        c.go.GetComponent<SpriteRenderer>().color = color_list[color_count];
                        color_count++;
                    }
                }
            }
        }

        ui_ctrl.GameMode();
        cam_ctrl.ResetCam();
        ui_ctrl.ResetUI();

        //If they quit and reloaded the game on a gameover screen
        if (state == GameState.lost)
        { 
            Lost();
        }

        //If they quit and reloaded the game on a win screen
        if (state == GameState.won)
        {
            Won();
        }
    }

    public void NewRound()
    {
        state = GameState.play;
        num_guesses = 0;
        SetGuessesAllowed();
        SetTimer();
        goal_country = GetRandomCountry();

        //Reset screen
        ui_ctrl.ResetUI();
        cam_ctrl.ResetCam();
        ui_ctrl.UpdateText();
        ui_ctrl.ResetGoalCountry();

        //Reset all country colors to white
        foreach (Country c in countryList)
        {
            c.go.GetComponent<SpriteRenderer>().color = Color.white;
        }

        //Clear guessed countries locally and in data
        guessed_countries.Clear();
        for(int i=0; i < menu_ctrl.data.guessed_coutries.Length; i++)
        {
            menu_ctrl.data.guessed_coutries[i] = "";
        }

        //Clear colors locally and in data
        color_list.Clear();
        for (int i = 0; i < menu_ctrl.data.country_colors.Length; i++)
        {
            menu_ctrl.data.country_colors[i] = Color.clear;
        }

        SaveGame();
    }


    void Update()
    {
        // Learn mode
        if (state == GameState.learn)
        {
            if (Input.touchCount == 1)
            {
                tap_timer += Time.deltaTime;
            }
            
            else if(tap_timer > 0.0f && tap_timer < 0.3f && !ui_ctrl.show_info)
            {
                tap_timer= 0.0f;
                if (goal_country != null)
                {
                    goal_country.go.GetComponent<SpriteRenderer>().color = Color.white;
                    ui_ctrl.ResetUI();
                }
                Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                goal_country = ClosestCountry(mouse_pos);
                goal_country.go.GetComponent<SpriteRenderer>().color = Color.green;
                MoveTo(goal_country);
            }
            else
            {
                tap_timer = 0.0f;
            }

            if (goal_country != null && cam_ctrl.isReady())
            {                 
                ui_ctrl.ShowAnswer();
            }
        }
        // Game mode
        // Search country list
        else if (state == GameState.play)
        {
            if (game_timer > 0.0f)
            {
                game_timer -= Time.deltaTime;
                ui_ctrl.SetTimerText(Mathf.Round(game_timer));
            }
            else if(game_timer <= 0.0f)
            {
                state = GameState.lost;
                Lost();
            }

            // Auto fill name
            bool foundAutoFillName = false;
            text = ui_ctrl.GetInput();
            int cursor_pos = ui_ctrl.input_field.GetComponent<TMP_InputField>().caretPosition;
            text = text.Substring(0, cursor_pos);
            if(text == "")
            {
                ui_ctrl.input_field.GetComponent<TMP_InputField>().text = "";
                return;
            }

            foreach(Country c in countryList)
            {
                if(text.Length > c.name.Length)
                {
                    continue;
                }
                if(c.name.Substring(0, cursor_pos).ToLower() == text.ToLower())
                {
                    int substr_len = c.name.Length - text.Length;
                    string total = text + c.name.Substring(text.Length, substr_len);
                    ui_ctrl.input_field.GetComponent<TMP_InputField>().text = total;
                    foundAutoFillName = true;
                    break;
                }
                if(text.Length > c.secondname.Length)
                {
                    continue;
                }
                if(c.secondname.Substring(0, cursor_pos).ToLower() == text.ToLower())
                {
                    int substr_len = c.secondname.Length - text.Length;
                    string total = text + c.secondname.Substring(text.Length, substr_len);
                    ui_ctrl.input_field.GetComponent<TMP_InputField>().text = total;
                    foundAutoFillName = true;
                    break;
                }
            }

            if (!foundAutoFillName)
            {
                ui_ctrl.input_field.GetComponent<TMP_InputField>().text = text;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                text = ui_ctrl.GetInput().ToLower();
                ui_ctrl.ResetGuessFeedback();

                while (text.EndsWith(" "))
                {
                    text = text.Remove(text.Length - 1, 1);
                }

                // If text is blank return
                if (text == "") return;

                // increment number of guesses
                num_guesses++;

                // If they got it right (checking nickname too)
                if (text == goal_country.name.ToLower() || text == goal_country.secondname.ToLower())
                {
                    streak++;
                    state = GameState.won;
                    SetScore();
                    Won();
                }
                // They got it wrong 
                else
                {
                    Country guess = null;
                    // Find game object associated with text
                    foreach (Country c in countryList)
                    {
                        if (c.name.ToLower() == text || c.secondname.ToLower() == text)
                        {
                            guess = c;
                            break;
                        }
                    }

                    // if they just misspelled
                    if (guess == null)
                    {
                        ui_ctrl.InvalidName();
                        num_guesses--;          //don't punish for misspelling
                    }
                    else
                    {
                        // If they are out of guesses
                        if (num_guesses == guesses_allowed)
                        {
                            state = GameState.lost;
                            Lost();
                        }
                        else
                        {
                            MoveTo(guess);
                            //calculate distance and change color based off of distance
                            float distance = CalculateDist(guess, goal_country);

                            Color newColor = GetColorFromDistance(distance);
                            guess.go.GetComponent<SpriteRenderer>().color = newColor;

                            if (guessed_countries.Contains(guess.name.ToLower()))
                            {
                                ui_ctrl.AlreadyGuessed();
                                num_guesses--;
                            }
                            else
                            {
                                guessed_countries.Add(guess.name.ToLower());
                                color_list.Add(newColor);
                                ui_ctrl.ResetInputField();
                            }
                        }
                    }
                }
            }
        }

        if((state == GameState.won || state == GameState.lost) && cam_ctrl.isReady())
        {
            ui_ctrl.ShowAnswer();
        }

        if (state != GameState.learn)
        {
            SaveGame();
            ui_ctrl.UpdateText();
        }
    }

    private void LateUpdate()
    {
        // In game camera stuff
        cam_ctrl.ZoomWithMouse();
        cam_ctrl.DragCam();
    }
    private void SetGuessesAllowed()
    {
        switch (menu_ctrl.data.mode_data)
        {
            case GameMode.Easy:
                guesses_allowed = MenuController.NumEasyGuess;
                break;

            case GameMode.Medium:
                guesses_allowed = MenuController.NumMedGuess;
                break;

            case GameMode.Hard:
                guesses_allowed = MenuController.NumHardGuess;
                break;

            case GameMode.Extreme:
                guesses_allowed = MenuController.NumExtremeGuess;
                break;
        }
    }

    private void SetTimer()
    {
        switch (menu_ctrl.data.mode_data)
        {
            case GameMode.Easy:
                game_timer = MenuController.EasyTime;
                break;

            case GameMode.Medium:
                game_timer = MenuController.MedTime;
                break;

            case GameMode.Hard:
                game_timer = MenuController.HardTime;
                break;

            case GameMode.Extreme:
                game_timer = MenuController.ExtremeTime;
                break;
        }
    }

    private void SetScore()
    {
        switch (menu_ctrl.data.mode_data)
        {
            case GameMode.Easy:
                score += MenuController.EastMult;
                break;

            case GameMode.Medium:
                score += MenuController.MedMult;
                break;

            case GameMode.Hard:
                score += MenuController.HardMult;
                break;

            case GameMode.Extreme:
                score += MenuController.ExtremeMult;
                break;
        }
    }
    float CalculateDist(Country guess, Country goal)
    {
        Vector2[] goal_vertices = goal.go.GetComponent<SpriteRenderer>().sprite.vertices;
        Vector2[] guess_vertices = guess.go.GetComponent<SpriteRenderer>().sprite.vertices;
        float minDistance = float.MaxValue;

        // Convert each vertex to world space and check distance between all other vertices. Return minimum distance
        foreach(Vector2 v1 in goal_vertices)
        {
            Vector2 point1 = goal.go.transform.TransformPoint(v1);
            foreach (Vector2 v2 in guess_vertices)
            {
                Vector2 point2 = guess.go.transform.TransformPoint(v2);
                float distance = Vector2.Distance(point1, point2);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        return minDistance;
    }

    float CalculateDist(Country a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.go.transform.position.x, a.go.transform.position.y), new Vector2(b.x, b.y));
    }

    Color GetColorFromDistance(float distance)
    {   
        float mappedDistance = Mathf.Clamp(distance, 0f, 10f) / 10f; // map distance to the range [0, 1]
        return color_scale[Mathf.RoundToInt(mappedDistance * (num_colors - 1))]; // get color from color scale based on mapped distance
    }

    // Generate the RGB color scale
    private void GenerateColorScale()
    {
        color_scale = new Color[num_colors];

        // Define the color stops for each color in the scale
        float redStop = 0.1f;
        float orangeStop = 0.2f;
        float yellowStop = 0.4f;
        float lightBlueStop = 0.6f;
        float blueStop = 0.8f;
        float darkBlueStop = 1.0f;

        // Define the colors
        Color darkRed = new Color(0.5f, 0f, 0f);
        Color orange = new Color(1.0f, 0.5f, 0.0f);
        Color darkBlue = new Color(0.0f, 0.0f, 0.5f);
        Color lightBlue = new Color(0.0f, 0.75f, 1.0f);

        for (int i = 0; i < num_colors; i++)
        {
            float t = i / (float)(num_colors - 1);

            Color color;

            // Interpolate the color between red and dark red
            if (t <= redStop)
            {
                color = Color.Lerp(darkRed, Color.red, t / redStop);
            }
            // Interpolate the color between red and orange
            else if (t <= orangeStop)
            {
                color = Color.Lerp(Color.red, orange, (t - redStop) / (orangeStop - redStop));
            }
            // Interpolate the color between orange and yellow
            else if (t <= yellowStop)
            {
                color = Color.Lerp(orange, Color.yellow, (t - orangeStop) / (yellowStop - orangeStop));
            }
            // Interpolate the color between yellow and light blue
            else if (t <= lightBlueStop)
            {
                color = Color.Lerp(Color.yellow, lightBlue, (t - yellowStop) / (lightBlueStop - yellowStop));
            }
            // Interpolate the color between light blue and blue
            else if (t <= blueStop)
            {
                color = Color.Lerp(lightBlue, Color.blue, (t - lightBlueStop) / (blueStop - lightBlueStop));
            }
            // Interpolate the color between blue and dark blue
            else if (t <= darkBlueStop)
            {
                color = Color.Lerp(Color.blue, darkBlue, (t - lightBlueStop) / (darkBlueStop - blueStop));
            }
            // If t is greater than 1, use dark blue as the color
            else
            {
                color = darkBlue;
            }

            color_scale[i] = color;
        }
    }


    private void SaveGame()
    {
        // Check if we have a new high score
        PlayerScore[] list = HighScores.instance.scoreList;
        if ((list.Length < 10 || score > list[list.Length - 1].score) && state == GameState.lost && !new_hs)
        {
            ui_ctrl.NewHighScore(true);
            new_hs = true;
        }
        
        // save the streak, score, number of guesses and goal country
        menu_ctrl.data.streak = streak;
        menu_ctrl.data.score = score;
        menu_ctrl.data.num_guesses = num_guesses;
        menu_ctrl.data.goal_country = goal_country.go;

        // if the player lost and there is a new high score, make sure they save the high score before starting new game
        if (state == GameState.lost)
        {
            if (new_hs)
            {
                if (hs_saved)
                {
                    menu_ctrl.data.game_in_progress = false;
                }
            }
            else
            {
                menu_ctrl.data.game_in_progress = false;
            }
        }

        // Save the game state
        switch (state)
        {
            case GameState.play:
                menu_ctrl.data.game_state = 0;
                break;

            case GameState.won:
                menu_ctrl.data.game_state = 1;
                break;

            case GameState.lost:
                menu_ctrl.data.game_state = 2;
                break;
        }

        // Save the countries already guesses
        for (int i = 0; i < guessed_countries.Count; i++)
        {
            menu_ctrl.data.guessed_coutries[i] = guessed_countries[i];
        }

        // Save their colors
        for (int i = 0; i < color_list.Count; i++)
        {
            menu_ctrl.data.country_colors[i] = color_list[i];
        }

        // Save data to file
        menu_ctrl.file_handler.Save(menu_ctrl.data);
    }
    public void SaveHighScore(string name)
    {
        ui_ctrl.OnMenuClick();
        PlayerPrefs.SetInt("highscore", score);
        HighScores.UploadScore(name, score);

        // Change bool so there's no longer a game in progress
        hs_saved = true;
    }

    private Country GetRandomCountry()
    {
        int r = Random.Range(0, countryList.Count);
        return countryList[r];
    }

    public void Lost()
    {
        state = GameState.lost;
        goal_country.go.GetComponent<SpriteRenderer>().color = Color.green;
        ui_ctrl.GameOver();
        MoveTo(goal_country);
    }

    private void Won()
    {
        goal_country.go.GetComponent<SpriteRenderer>().color = Color.green;
        ui_ctrl.Won();
        MoveTo(goal_country);
    }

    private void MoveTo(Country c)
    {
        //Move camera to country region
        cam_ctrl.MoveTo(new Vector3(c.go.transform.position.x, c.go.transform.position.y, -1f));

        //Zoom in
        cam_ctrl.ZoomIn();
    }

    private Country ClosestCountry(Vector3 pos)
    {
        float closestDistance = 10000f;
        Country closestCountry = countryList[0];
        foreach(Country c in countryList)
        {
            float distance = CalculateDist(c, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCountry = c;
            }
        }

        return closestCountry;
    }


}
