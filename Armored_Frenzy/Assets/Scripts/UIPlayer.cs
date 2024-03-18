using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public enum PlayerState { Win, Lose, Active}

public class UIPlayer : MonoBehaviour
{
    private PlayerControls playerControls;
    InputAction Select;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Slider EnergyBar;

    [SerializeField]
    private TextMeshProUGUI EnergyText;

    [SerializeField]
    private TextMeshProUGUI PlayerStateText;

    [SerializeField]
    private Image BoostIcon;

    private int DefaultEnergy;

    public static PlayerState state;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.NewPlayer.ResetButton.performed += ResetGame;
        
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

        DefaultEnergy = player.Energy;

        EnergyBar.maxValue = player.Energy;
        EnergyBar.minValue = 0;

        state = PlayerState.Active;

        BoostIcon.gameObject.SetActive(false);

        PlayerStateText.text = "";
        PlayerStateText.gameObject.SetActive(false);
        EnergyText.text = SetText();
    }

    // Update is called once per frame
    void Update()
    {
        EnergyText.text = SetText();

        EnergyBar.value = player.Energy;

        PlayerGameState();

        bool SetBoostIcon = player.Inventory[0] != null ? true : false;

        BoostIcon.gameObject.SetActive(SetBoostIcon);
       

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

        string temp = $"Energy: {tempEnergy}/{DefaultEnergy}"; //only player.Energy should change

        return temp;
    }

    private void PlayerGameState()
    {
        switch(state)
        {
            
            case PlayerState.Lose:
                Time.timeScale = 0;
                PlayerStateText.gameObject.SetActive(true);
                LoseState();
                
                break;
            case PlayerState.Win:
                Time.timeScale = 0;
                PlayerStateText.gameObject.SetActive(true);
                WinState();
                
                break;
            case PlayerState.Active:
                Time.timeScale = 1;
                break;
        }
    }

    private void LoseState()
    {
        PlayerStateText.text = SetState("Lose");
    }

    public void WinState()
    {
        PlayerStateText.text = SetState("Win!");
    }

    private string SetState(string condition)
    {
        return $"You {condition} \n Press A to retry";
    }

    private void ResetGame(InputAction.CallbackContext callback)
    {
        if(state == PlayerState.Win || state == PlayerState.Lose)
        {
            //Issue with bullets 
            SceneManager.LoadScene(0);
        }
        
    }
}
