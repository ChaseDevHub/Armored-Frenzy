using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScoreData : MonoBehaviour
{
    /*
     Types: Whole Numbers
     Red Cylinder: 50 each
     Pass through Energy Rings: 75 each
     Multiply by energy left over

     Formula: (RC * 50) + (ER * 75) 
    * EnergyBarLeftOver = Score
     */

    static int TotalScore;

    private static int DestroyedObjCount;

    private static int EnergyReplenishsCount;

    private static int EnergyBarRemainingCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Destroyed objects "+DestroyedObjCount);
        //Debug.Log("Pass by Ring " + EnergyReplenishsCount);
        //Debug.Log("EnergyTotal " + EnergyBarRemainingCount);
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
        Debug.Log("Destroyed objects " + DestroyedObjCount);
        Debug.Log("Pass by Ring " + EnergyReplenishsCount);
        Debug.Log("EnergyTotal " + EnergyBarRemainingCount);

        //DestroyedObjCount *= 50;
        //EnergyReplenishsCount *= 75;

        TotalScore = (DestroyedObjCount * 50) + (EnergyReplenishsCount * 75) * EnergyBarRemainingCount;

        Debug.Log("Score " + TotalScore);
    }
}
