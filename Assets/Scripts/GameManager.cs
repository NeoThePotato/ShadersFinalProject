using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Slider accuracyScoreSlider;
    [SerializeField] private int maxAccuracyScore = 100;
    [SerializeField] private int minAccuracyScore = 0;

    private int currentAccuracyScore = 0;
    
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        int newScore = EvaluateAccuracy();
        if (currentAccuracyScore != newScore)
        {
            currentAccuracyScore = newScore;
            accuracyScoreSlider.value = Mathf.Clamp(currentAccuracyScore, minAccuracyScore, maxAccuracyScore);

            if (currentAccuracyScore == maxAccuracyScore)
            {
                WinTheGame();
            }
        }
    }

    public int EvaluateAccuracy()
    {
        int accuracyScore = 0;
        //TODO: Evaluate the accuracy
        return accuracyScore;
    }

    private void WinTheGame()
    {
        //TODO: Add fanfare
    }
}
