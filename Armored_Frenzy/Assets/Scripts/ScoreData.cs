using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScoreData : MonoBehaviour
{
    static int TotalScore;

    private static int DestroyedObjCount;

    private static int EnergyReplenishsCount;

    private static int EnergyBarRemainingCount;

    //Not Needing Start and Update methods

    void Start()
    {
        DestroyedObjCount = 0;
        EnergyReplenishsCount = 0;
        EnergyBarRemainingCount = 0;
    }

    public static void AddDestroyedObjCount(int num)
    {
        DestroyedObjCount += num;
    }

    public static void AddEnergyRingCount(int num)
    {
        EnergyReplenishsCount += num;
    }

    public static void GetEnergyCount(int value)
    {
        EnergyBarRemainingCount = value;
    }

    public static void CalculateScore()
    {
        TotalScore = ((DestroyedObjCount * 50) + (EnergyReplenishsCount * 75) + EnergyBarRemainingCount) * 10;
    }

    public static int DataInfo(int num)
    {
        int output = 0;

        switch (num)
        {
            case 1:
                output = DestroyedObjCount;
                break;
            case 2:
                output = EnergyReplenishsCount;
                break;
            case 3:
                output = EnergyBarRemainingCount;
                break;
            case 4:
                output = TotalScore;
                break;
        }

        return output;
    }
}
