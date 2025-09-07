using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static System.Action<int> OnScoreChanged;
    public static System.Action OnGameOver;
    public static System.Action OnGameStart;
}