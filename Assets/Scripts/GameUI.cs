using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject input_field;

    // Buttons

        // Main
        public Button main_menu;
        public Button give_up;
        public Button help;

        // End of round
        public Button next_round;
        public Button restart;
        public Button info_button;
    

        // New high score
        public Button up1;
        public Button up2;
        public Button up3;
        public Button up4;
        public Button down1;
        public Button down2;
        public Button down3;
        public Button down4;
        public Button save_name;

        // Misc
        public Button affirm_text_box;
        public Button affirm_give_up;
        public Button cancel_give_up;
        public Button invalid_name;
        public Button already_guessed;
        public Button close_info_box;

    // Text
    public GameObject guesses_text;
    public GameObject streak_text;
    public GameObject game_over_text;
    public GameObject info_text;
    public GameObject answer_text;
    public GameObject char1;
    public GameObject char2;
    public GameObject char3;
    public GameObject char4;
    public GameObject please_save;
    public GameObject timer_text;
    public GameObject score_text;

    // Boxes
    public GameObject info_box;
    public GameObject guess_box;
    public GameObject answer_box;
    public GameObject new_hs_box;

    // MISC
    public ParticleSystem confetti;
    public GameObject flag_placeholder;
    public GameObject menu;
    public bool show_info;

    // Private
    private CountryFinder cf;
    private CamController cam_ctrl;
    private char[] hs_name;
    


    void Awake()
    {
        //Add listeners
        main_menu.onClick.AddListener(OnMenuClick);
        next_round.onClick.AddListener(OnNextRoundClick);
        restart.onClick.AddListener(OnRestartClick);
        give_up.onClick.AddListener(OnGiveUpClick);
        affirm_give_up.onClick.AddListener(OnAffirmGiveUpClick);
        cancel_give_up.onClick.AddListener(OnCancelGiveUpClick);
        cf = gameObject.GetComponent<CountryFinder>();
        cam_ctrl = Camera.main.GetComponent<CamController>();
        info_button.onClick.AddListener(OnInfoButtonClick);
        close_info_box.onClick.AddListener(OnCloseInfoClick);

        save_name.onClick.AddListener(OnSaveNameClick);
        up1.onClick.AddListener(OnUp1Click);
        up2.onClick.AddListener(OnUp2Click);
        up3.onClick.AddListener(OnUp3Click);
        up4.onClick.AddListener(OnUp4Click);

        down1.onClick.AddListener(OnDown1Click);
        down2.onClick.AddListener(OnDown2Click);
        down3.onClick.AddListener(OnDown3Click);
        down4.onClick.AddListener(OnDown4Click);

        hs_name = new char[4] {'A', 'A', 'A', 'A'};
    }
    public void ResetUI()
    {
        ResetInputField();
        game_over_text.SetActive(false);
        restart.gameObject.SetActive(false);
        next_round.gameObject.SetActive(false);
        answer_box.SetActive(false);
        confetti.gameObject.SetActive(false);
        info_text.SetActive(false);
        invalid_name.gameObject.SetActive(false);
        already_guessed.gameObject.SetActive(false);
        info_button.gameObject.SetActive(false);
    }

    public void ResetGuessFeedback()
    {
        invalid_name.gameObject.SetActive(false);
        already_guessed.gameObject.SetActive(false);
    }

    public void UpdateText()
    {
        guesses_text.GetComponent<TMP_Text>().text = (cf.guesses_allowed - cf.num_guesses).ToString();
        streak_text.GetComponent<TMP_Text>().text = "Streak: " + cf.streak.ToString();
        score_text.GetComponent<TMP_Text>().text = "Score: " + cf.score.ToString();
    }
    public string GetInput()
    {
        if (input_field.GetComponent<TMP_InputField>().text == null)
        {
            return "";
        }
        return input_field.GetComponent<TMP_InputField>().text;
    }

    public void ResetInputField()
    {
        input_field.GetComponent<TMP_InputField>().ActivateInputField();
        input_field.GetComponent<TMP_InputField>().text = "";
    }
    public void InvalidName()
    {
        invalid_name.gameObject.SetActive(true);
    }
    public void AlreadyGuessed()
    {
        already_guessed.gameObject.SetActive(true);
    }
    public void OnMenuClick()
    {
        gameObject.SetActive(false);
        menu.SetActive(true);

        cam_ctrl.SetPosition(new Vector3(0.0f, 0.0f, -1f));
        cam_ctrl.ZoomOut();
        cam_ctrl.SetBackground(Color.black);

        ResetUI();
    }

    public void GameOver()
    {
        restart.gameObject.SetActive(true);
        game_over_text.SetActive(true);
        info_button.gameObject.SetActive(true);
        timer_text.SetActive(false);
    }
    public void Won()
    {
        streak_text.GetComponent<TMP_Text>().text = "Streak: " + cf.streak.ToString();
        next_round.gameObject.SetActive(true);
        confetti.gameObject.transform.position = new Vector3(cf.goal_country.go.transform.position.x, cf.goal_country.go.transform.position.y + 5f);
        confetti.gameObject.SetActive(true);
        info_button.gameObject.SetActive(true);
    }
    public void ShowAnswer()
    {
        answer_box.gameObject.SetActive(true);
        info_button.gameObject.SetActive(true);
        flag_placeholder.GetComponent<Image>().sprite = cf.goal_country.flag.GetComponent<SpriteRenderer>().sprite;

        string answer = cf.goal_country.name;
        if (cf.goal_country.secondname != "")
        {
            answer += " (" + cf.goal_country.secondname + ")";
        }
        answer_text.GetComponent<TMP_Text>().text = answer;
    }

    public void LearnMode()
    {
        input_field.SetActive(false);
        guesses_text.SetActive(false);
        streak_text.SetActive(false);
        give_up.gameObject.SetActive(false);
        help.gameObject.SetActive(false);
        guess_box.SetActive(false);
        timer_text.SetActive(false);
        answer_box.SetActive(false);
        timer_text.SetActive(false);
        score_text.SetActive(false);
        ResetUI();
    }

    public void GameMode()
    {
        UpdateText();
        input_field.SetActive(true);
        guesses_text.SetActive(true);
        streak_text.SetActive(true);
        give_up.gameObject.SetActive(true);
        help.gameObject.SetActive(true);
        guess_box.SetActive(true);
        timer_text.SetActive(true);
        score_text.SetActive(true);
    }

    public void ResetGoalCountry()
    {
        if (cf.goal_country != null)
        {
            cf.goal_country.go.GetComponent<SpriteRenderer>().color = Color.white;
            cf.goal_country.flag.SetActive(false);
        }
    }

    public void NewHighScore(bool active)
    {
        new_hs_box.SetActive(active);
        save_name.gameObject.SetActive(active);
    }

    public void SetTimerText(float timer)
    {
        timer_text.GetComponent<TMP_Text>().text = timer.ToString();
    }

    private void OnSaveNameClick()
    {
        NewHighScore(false);
        string name = "" + hs_name[0] + hs_name[1] + hs_name[2] + hs_name[3];
        cf.SaveHighScore(name);
    }
    private void OnUp1Click()
    {
        hs_name[0] = MoveUp(hs_name[0]);
        char1.GetComponent<TMP_Text>().text = "" + hs_name[0];
    }
    private void OnUp2Click()
    {
        hs_name[1] = MoveUp(hs_name[1]);
        char2.GetComponent<TMP_Text>().text = "" + hs_name[1];
    }
    private void OnUp3Click() 
    {
        hs_name[2] = MoveUp(hs_name[2]);
        char3.GetComponent<TMP_Text>().text = "" + hs_name[2];
    }
    private void OnUp4Click() 
    {
        hs_name[3] = MoveUp(hs_name[3]);
        char4.GetComponent<TMP_Text>().text = "" + hs_name[3];
    }

    private void OnDown1Click() 
    {
        hs_name[0] = MoveDown(hs_name[0]);
        char1.GetComponent<TMP_Text>().text = "" + hs_name[0];
    }
    private void OnDown2Click() 
    {
        hs_name[1] = MoveDown(hs_name[1]);
        char2.GetComponent<TMP_Text>().text = "" + hs_name[1];
    }
    private void OnDown3Click() 
    {
        hs_name[2] = MoveDown(hs_name[2]);
        char3.GetComponent<TMP_Text>().text = "" + hs_name[2];
    }
    private void OnDown4Click() 
    {
        hs_name[3] = MoveDown(hs_name[3]);
        char4.GetComponent<TMP_Text>().text = "" + hs_name[3];
    }

    private char MoveUp(char s)
    {
        if (s == 'A') return 'Z';
        int ascii = Convert.ToByte(s);
        ascii -= 1;
        return Convert.ToChar(ascii);
    }

    private char MoveDown(char s)
    {
        if (s == 'Z') return 'A';
        int ascii = Convert.ToByte(s);
        ascii += 1;
        return Convert.ToChar(ascii);
    }

    private void OnNextRoundClick()
    {
        cf.NewRound();
    }
    private void OnRestartClick()
    {
        cf.StartNewGame();
        NewHighScore(false);
    }

    private void OnInfoButtonClick()
    {
        show_info = true;
        string text = "";
        if (cf.goal_country.infoList[6].year != 0)
        {
            text += "Population: " + cf.goal_country.infoList[6].value + " (million) (" + cf.goal_country.infoList[6].year + ")\n\n";
        }
        else
        {
            text += "Population: N/A\n\n";
        }
        if (cf.goal_country.infoList[5].year != 0)
        {
            text += "Population density per square km: " + cf.goal_country.infoList[5].value + " (" + cf.goal_country.infoList[5].year + ")\n\n";
        }
        else
        {
            text += "Population density per square km: N/A\n\n";
        }
        if (cf.goal_country.infoList[7].year != 0)
        {
            text += "Land area: " + cf.goal_country.infoList[7].value + " (thousand hectares) (" + cf.goal_country.infoList[7].year + ")\n\n";
        }
        else
        {
            text += "Land area: N/A\n\n";
        }
        if (cf.goal_country.infoList[0].year != 0)
        {
            text += "CO2 Emissions: " + cf.goal_country.infoList[0].value + "  (thousand metric tons) (" + cf.goal_country.infoList[0].year + ")\n\n";
        }
        else
        {
            text += "CO2 Emissions: N/A\n\n";
        }
        if (cf.goal_country.infoList[2].year != 0)
        {
            text += "Students enrolled in upper secondary education: " + cf.goal_country.infoList[2].value + " (thousand) (" + cf.goal_country.infoList[2].year + ")\n\n";
        }
        else
        {
            text += "Students enrolled in upper secondary education: N/A\n\n";
        }
        if (cf.goal_country.infoList[3].year != 0)
        {
            text += "GDP in current prices: " + cf.goal_country.infoList[3].value + " (millions of US dollars) (" + cf.goal_country.infoList[3].year + ")\n\n";
        }
        else
        {
            text += "GDP in current prices: N/A\n\n";
        }
        if (cf.goal_country.infoList[4].year != 0)
        {
            text += "Unemployment rate: " + cf.goal_country.infoList[4].value + "% (" + cf.goal_country.infoList[4].year + ")\n\n";
        }
        else
        {
            text += "Unemployment rate: N/A\n\n";
        }
        if (cf.goal_country.infoList[8].year != 0)
        {
            text += "Individuals using the internet: " + cf.goal_country.infoList[8].value + "% (" + cf.goal_country.infoList[8].year + ")\n\n";
        }
        else
        {
            text += "Individuals using the internet: N/A\n\n";
        }
        if (cf.goal_country.infoList[1].year != 0)
        {
            text += "Intentional homicide rates per 100,000: " + cf.goal_country.infoList[1].value + " (" + cf.goal_country.infoList[1].year + ")";
        }
        else
        {
            text += "Intentional homicide rates per 100,000: N/A";
        }

        info_text.GetComponent<TMP_Text>().text = text;
    }

    private void OnCloseInfoClick()
    {
        Invoke(nameof(TurnOffInfo), 1f);
    }

    private void TurnOffInfo()
    {
        show_info = false;
    }
    private void OnGiveUpClick()
    {
        if (cf.state == GameState.play)
        {
            affirm_text_box.gameObject.SetActive(true);
        }
    }
    private void OnCancelGiveUpClick()
    {
        affirm_text_box.gameObject.SetActive(false);
    }
    private void OnAffirmGiveUpClick()
    {
        affirm_text_box.gameObject.SetActive(false);
        cf.Lost();
    }

    private void TurnOffPleaseSave()
    {
        please_save.SetActive(false);
    }
}
