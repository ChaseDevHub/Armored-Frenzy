using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System;

public enum PlayerState { Win, Lose, Active, Wait}

public class UIPlayer : MonoBehaviour
{
    private PlayerControls playerControls;

    private Player player;

    private Slider EnergyBar;

    private TextMeshProUGUI EnergyText;

    public static TextMeshProUGUI PlayerStateText;
    
    private TextMeshProUGUI TimerText;

    private Image BoostIcon;

    private Image FadePanel;

    float FadeTime;

    private int DefaultEnergy;

    public static PlayerState state; //{ get; set; }

    public static bool StartTimer;

    private float SetTime = 120f; //reads in seconds

    int seconds;
    int minutes;

    [SerializeField]
    private Audio[] audios;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.NewPlayer.ResetButton.performed += ResetGame;
        
    }

    private void OnDisable()
    {
        playerControls.NewPlayer.ResetButton.performed -= ResetGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindAnyObjectByType<Player>();
        }
        if(EnergyBar == null)
        {
            EnergyBar = GameObject.FindAnyObjectByType<Slider>();
        }
        if (EnergyText == null)
        {
            EnergyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        }
        if(BoostIcon == null)
        {
            BoostIcon= GameObject.Find("BoostIcon").GetComponent<Image>();
        }
        if(PlayerStateText == null)
        {
            PlayerStateText = GameObject.Find("PlayerStateText").GetComponent<TextMeshProUGUI>();
        }
        if(TimerText == null)
        {
            TimerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        }
        if(FadePanel == null)
        {
            FadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
        }
    

        DefaultEnergy = player.Energy;

        EnergyBar.maxValue = player.Energy;
        EnergyBar.minValue = 0;

        state = PlayerState.Wait;

        BoostIcon.gameObject.SetActive(false);

        PlayerStateText.text = "";
        PlayerStateText.gameObject.SetActive(false);
        EnergyText.text = SetText();
        
        StartTimer = false;
        

        FadePanel.color = new Color(0, 0, 0, 0);
        FadeTime = 0;

        seconds = ((int)SetTime % 60);
        minutes = ((int)SetTime / 60);
        TimerText.text = string.Format($"{minutes.ToString("00")}:{seconds.ToString("00")}");
    }

    // Update is called once per frame
    void Update()
    {
        EnergyText.text = SetText();

        EnergyBar.value = player.Energy;
        EnergySound();

        PlayerGameState();

        bool SetBoostIcon = player.Inventory[0] != null ? true : false;

        BoostIcon.gameObject.SetActive(SetBoostIcon);
       
        if(StartTimer)
        {
            TimerText.text = Timer();
        }

        if(!player.PlayerInControl && FadeTime < 225)
        {
            FadeTime += 1f * Time.deltaTime;
            FadePanel.color = new Color(0, 0, 0, FadeTime);
        }
        else if(player.PlayerInControl && FadeTime > 0)
        {
            FadeTime -= 1 * Time.deltaTime;
            FadePanel.color = new Color(0, 0, 0, FadeTime);
        }
    }

    private string SetText()
    {
        float tempEnergy;

        if(player.Energy <= 0)
        {
            tempEnergy = 0;
            state = PlayerState.Lose;
        }
        else
        {
            tempEnergy = player.Energy;
        }
        //Energy: 
        string temp = $"{tempEnergy}/{DefaultEnergy}"; //only player.Energy should change

        return temp;
    }

    private void PlayerGameState()
    {
        switch(state)
        {
            
            case PlayerState.Lose:
                //Time.timeScale = 0;
                PlayerStateText.gameObject.SetActive(true);
                player.StopPlayer();
                LoseState();
                
                break;
            case PlayerState.Win:
                //Time.timeScale = 0;
                ScoreData.GetEnergyCount(player.Energy);
                PlayerStateText.gameObject.SetActive(true);
                player.StopPlayer();
                WinState();
                
                break;
            case PlayerState.Active:
            case PlayerState.Wait:
                //Time.timeScale = 1;
                break;

        }
    }

    private void LoseState()
    {
        PlayerStateText.text = SetState("Lose");
    }

    private bool ReadScore = false;

    public void WinState()
    {
        PlayerStateText.text = SetState("Win!");
        if(!ReadScore)
        {
            ScoreData.CalculateScore();
            ReadScore= true;
        }
        
    }

    private string SetState(string condition)
    {
        string output = $"You {condition}";

        if(state == PlayerState.Lose)
        {
            output += " \nPress \nto retry";
        }

        return output;
    }

    private string Timer()
    {
        if(state == PlayerState.Active)
        {
            if (StartTimer)
            {
                SetTime -= Time.deltaTime;
            }


            if (SetTime < 0)
            {
                SetTime = 0;
                state = PlayerState.Lose;
            }

            //help from https://forum.unity.com/threads/timer-with-string-format.1276847/
            seconds = ((int)SetTime % 60);
            minutes = ((int)SetTime / 60);
        }
        

        //help from https://stackoverflow.com/questions/18361301/get-a-01-from-an-int-instead-of-1-in-a-for-loop
        return string.Format($"{minutes.ToString("00")}:{seconds.ToString("00")}");
    }

    private void ResetGame(InputAction.CallbackContext callback)
    {
        if(state == PlayerState.Win || state == PlayerState.Lose)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
    
    private void EnergySound()
    {
        switch(EnergyBar.value)
        {
            case 5:
                PlayEnergyAudio(0);
                break;
            case 4:
                PlayEnergyAudio(1);
                break;
            case 3:
                PlayEnergyAudio(2);
                break;
            case 2:
                PlayEnergyAudio(3);
                break;
            case 1:
                PlayEnergyAudio(4);
                break;
            default:
                foreach (var v in audios)
                {
                    v.StopAudio();
                }
                break;

        }
    }

    private void PlayEnergyAudio(int num)
    {
        for(int i = 0; i < audios.Length; i++)
        {
            if (audios[i].audioName.isPlaying && i != num)
            {
                audios[i].StopAudio(); 
            }
            
            if (!audios[num].audioName.isPlaying)
            {
                audios[num].PlayAudio();
            }
        }

        
    }
}
