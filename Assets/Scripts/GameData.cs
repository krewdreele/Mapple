using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int streak;
    public int num_guesses;
    public string[] guessed_coutries;
    public Color[] country_colors;
    public GameObject goal_country;
    public int game_state;
    public bool game_in_progress;
    public GameMode mode_data;
    public int score;
    public GameData()
    {
        mode_data = GameMode.None;
        streak = 0;
        num_guesses = 0;
        guessed_coutries = new string[MenuController.MaxGuessesAllowed];
        country_colors = new Color[MenuController.MaxGuessesAllowed];
        goal_country = null;
        game_state = 0;
        game_in_progress = false;
        score = 0;
        for(int i = 0; i < guessed_coutries.Length; i++)
        {
            guessed_coutries[i] = "";
        }

        for(int j = 0; j < country_colors.Length; j++)
        {
            country_colors[j] = Color.clear;
        }
    }
}

[System.Serializable]
public class Country
{
    public GameObject go;
    public string name;
    public string secondname;
    public List<Info> infoList;
    public GameObject flag;

    public Country(GameObject go, string name, string secondname)
    {
        this.go = go;
        this.name = name;
        infoList = new List<Info>();
        this.secondname = secondname;
    }   
}

public class Info 
{
    public int year;
    public float value;

    public Info(int year, float value)
    {
        this.year = year;
        this.value = value;
    }
}

