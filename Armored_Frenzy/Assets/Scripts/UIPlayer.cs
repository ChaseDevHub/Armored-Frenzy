using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerState { Win, Lose, Active}

public class UIPlayer : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private Slider EnergyBar;

    [SerializeField]
    private TextMeshProUGUI EnergyText;

    private int DefaultEnergy;

    private PlayerState state;
   
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
            EnergyText = GameObject.FindAnyObjectByType<TextMeshProUGUI>();
        }

        DefaultEnergy = player.Energy;

        EnergyBar.maxValue = player.Energy;
        EnergyBar.minValue = 0;

        state = PlayerState.Active;
    }

    // Update is called once per frame
    void Update()
    {
        EnergyText.text = SetText();

        EnergyBar.value = player.Energy;

        PlayerGameState();
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
            //Replace the debug logs with proper endings as project moves forward
            case PlayerState.Lose:
                LoseState();
                break;
            case PlayerState.Win:
                Debug.Log("Win");
                break;
            case PlayerState.Active:
                Debug.Log("Player is active in game"); //This might not be needed
                break;
        }
    }

    private void LoseState()
    {
        Debug.Log("Lose");
        //Screen stops for all movement (like a pause)
        //Have this be the place where the player has the ability to restart
        //Temp text for another day
    }
}
