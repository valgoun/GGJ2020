﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Master : MonoBehaviour
{
    public List<Key> AllowedEasyInputs;
    public List<Key> AllowedMediumInputs;

    public Image FadeToBlack;
    public float FadeTime;

    [NonSerialized] public int EnemyKilled;
    [NonSerialized] public int InputCollected;
    [NonSerialized] public float GameStart;
    [NonSerialized] public bool GameOverState;
    [NonSerialized] public List<Key> CurrentInputs;
    [NonSerialized] public int EasyStart;

    static Master _instance;
    public static Master Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Master();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance)
            Destroy(gameObject);
        else
            _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StartGame(int index)
    {
        GameStart = Time.time;
        EnemyKilled = 0;
        InputCollected = 0;
        GameOverState = false;
        EasyStart = index;

        if (index == 0)
            CurrentInputs = AllowedEasyInputs;
        else if (index == 1)
            CurrentInputs = AllowedMediumInputs;
        else
            CurrentInputs = new List<Key>();

        SceneManager.LoadScene("Main");
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        GameOverState = true;
        GameStart = Time.time - GameStart;

        if (FadeToBlack)
            StartCoroutine(GameOverRoutine());
        else
            GoToMainMenu();
    }

    IEnumerator GameOverRoutine ()
    {
        float timestamp = Time.fixedDeltaTime;
        Color color = FadeToBlack.color;
        while (timestamp + FadeTime > Time.fixedDeltaTime)
        {
            color.a = (Time.fixedDeltaTime - timestamp) / FadeTime;
            FadeToBlack.color = color;

            yield return null;
        }

        color.a = 1;
        FadeToBlack.color = color;

        GoToMainMenu();
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
