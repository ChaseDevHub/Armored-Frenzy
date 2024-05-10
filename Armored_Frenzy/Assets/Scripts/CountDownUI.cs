using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI CountDown;

    float VisibleColorTime;

    //bool ShowTimer;
    bool StartCountDown;

    float TimeRemain;

    int ColorValue;

    [SerializeField]
    float TimerValue;

    string CountText;

    [SerializeField]
    private Audio GameMusic;


    // Start is called before the first frame update
    void Start()
    {
        ColorValue = 255;
        if (CountDown == null)
        {
            CountDown = GameObject.Find("CountDownTimer").GetComponent<TextMeshProUGUI>();
        }

        //ShowTimer = false;

        CountDown.color = new Color(ColorValue, ColorValue, ColorValue, 0);

        TimeRemain = 1;

        TimerValue = TimerValue == 0 ? 3 : TimerValue;

        CountDown.text = $"{TimerValue.ToString("0")}";

        StartCountDown = false;
        VisibleColorTime = 0;

        CountText = "";

        GameMusic.PlayAudio();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(VisibleColorTime);
        StartCountdown();       
    }

    private void StartCountdown()
    {
        if(TimeRemain > 0)
        {
            TimeRemain -= Time.deltaTime;
        }
        else
        {
            if (!StartCountDown)
            {
                if (VisibleColorTime <= 2)
                {
                    VisibleColorTime += 1f * Time.deltaTime;
                    CountDown.color = new Color(ColorValue, ColorValue, ColorValue, VisibleColorTime);
                }
                else
                {
                    StartCountDown = true;
                }
            }
            else //if (ShowTimer && StartCountDown)
            {
                if(UIPlayer.state == PlayerState.Wait)
                {
                    CountDown.text = ChangeText(CountText);
                }
                
            }
        }
       
    }

    private string ChangeText(string output)
    {
        //string output = $"{TimerValue.ToString("0")}";
        if(TimerValue > 0)
        {
            TimerValue -= Time.deltaTime;
            if(TimerValue < 1)
            {
                output = $"G.0!";
            }
            else
            {
                output = $"{TimerValue.ToString("0")}";
            }   
        }
        else
        {
            UIPlayer.StartTimer = true;
            UIPlayer.state = PlayerState.Active;
            output = "";
        }

        return output;

    }

    //UIPlayer.StartTimer = true;
}
