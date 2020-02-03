﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("References")]
    public GameObject Main;
    public GameObject GameOver;
    public GameObject Difficulty;

    [Header("GameOver")]
    public TextMeshProUGUI Stats;

    GameObject _activeObject;
    
    // Start is called before the first frame update
    void Start()
    {
        _activeObject = Main;
        Time.timeScale = 1;

        if (Master.Instance.GameOverState)
            OpenGameOver();

        Master.Instance.MenuLoaded();
    }

    public void StartGame(int index)
    {
        SoundManager.Instance.PlaySound(SoundTypes.MENU_SELECTION);

        _activeObject.SetActive(false);
        Master.Instance.StartGame(index);
    }

    void OpenGameOver()
    {
        _activeObject.SetActive(false);
        GameOver.SetActive(true);
        _activeObject = GameOver;

        Stats.text = Mathf.FloorToInt(Master.Instance.GameStart) + "s\n" + Master.Instance.EnemyKilled + "\n" + Master.Instance.InputCollected;
    }

    public void OpenDifficulty ()
    {
        SoundManager.Instance.PlaySound(SoundTypes.MENU_SELECTION);

        _activeObject.SetActive(false);
        Difficulty.SetActive(true);
        _activeObject = Difficulty;
    }

    public void ReturnToMainMenu()
    {
        SoundManager.Instance.PlaySound(SoundTypes.MENU_SELECTION);

        _activeObject.SetActive(false);
        Main.SetActive(true);
        _activeObject = Main;
    }

    public void Quit()
    {
        Application.Quit();
    }
}