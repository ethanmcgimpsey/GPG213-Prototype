using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public int startMoney = 100;

    public static int Rounds;
    public int startRound = 1;

    public static int Lives;
    public int startLives = 100;

    public static int Score;
    public int scorePoints = 0;
    void Start()
    {
        Money = startMoney;
        Lives = startLives;
        Rounds = startRound;
        Score = scorePoints;
    }
}
