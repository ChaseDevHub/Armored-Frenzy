using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum GameState { Active, Pause}

public class UIPause : MonoBehaviour
{
    private PlayerControls playerControls;

    [SerializeField]
    private Button[] ButtonPausedOptions;

    [SerializeField]
    private GameObject PauseUI;

    private GameState state;

    //private int PauseID;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.NewPlayer.PauseControl.performed += PauseTheGame;
        playerControls.NewPlayer.Boost.performed += UnpauseTheGame;

    }

    private void OnDisable()
    {
        playerControls.NewPlayer.PauseControl.performed -= PauseTheGame;
        playerControls.NewPlayer.Boost.performed -= UnpauseTheGame;

    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Active;
        
        ButtonPausedOptions = new Button[2];

        if (ButtonPausedOptions[0] == null)
        {
            ButtonPausedOptions[0] = GameObject.Find("ReturnBTN").GetComponent<Button>();
        }

        if (ButtonPausedOptions[1] == null)
        {
            ButtonPausedOptions[1] = GameObject.Find("ExitBTN").GetComponent<Button>();
        }

        ButtonPausedOptions[0].Select();
        
        for (int i = 0; i < ButtonPausedOptions.Length; i++)
        {
            int buttonNum = i;
            ButtonPausedOptions[i].onClick.AddListener(() => ButtonResponce(buttonNum));
        }

        if(PauseUI == null)
        {
            PauseUI = GameObject.Find("PauseUI");
        }

        PauseUI.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        SetGameStatus();
    }

    private void SetGameStatus()
    {
        switch(state)
        {
            case GameState.Active:
                Time.timeScale = 1;
                PauseUI.SetActive(false);
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                PauseUI.SetActive(true);
                break;
        }
    }

    private void ButtonResponce(int button)
    {
        switch(button)
        {
            case 0:
                //state = GameState.Active;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case 1:
                SceneManager.LoadScene(0);
                break;
        }
    }

    private void PauseTheGame(InputAction.CallbackContext callback)
    {
        state = GameState.Pause;
        ButtonPausedOptions[0].Select();
    }

    private void UnpauseTheGame(InputAction.CallbackContext callback)
    {
        if(state == GameState.Pause)
        {
            state = GameState.Active;
        }
    }    

}
