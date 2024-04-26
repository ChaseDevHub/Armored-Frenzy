using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    private PlayerControls playerControls;
    private InputAction SelectControl;

    [SerializeField]
    private GameObject MenuUI;

    [SerializeField]
    private GameObject ControlsUI;

    [SerializeField]
    private GameObject ObjectiveUI;

    [SerializeField]
    private TextMeshProUGUI ControlsText;

    [SerializeField]
    private Button[] ButtonTrackOption;

    [SerializeField]
    bool StartGame;

    public static Controls control;

    float condition;

    bool ChangeScene;
    bool ReturnScene;

    [SerializeField]
    public static int TrackID;

    [SerializeField]
    private Audio MainMenuIdle;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.NewPlayer.Acceleration.performed += SelectToGoToNextScreen;
        playerControls.NewPlayer.Boost.performed += ReturnToPreviousScreen;
    }

    private void OnEnable()
    {
        SelectControl = playerControls.NewPlayer.SelectControl;
        SelectControl.Enable();
    }

    private void OnDisable()
    {
       SelectControl.Disable();
       playerControls.NewPlayer.Acceleration.performed -= SelectToGoToNextScreen;
       playerControls.NewPlayer.Boost.performed -= ReturnToPreviousScreen;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(ControlsText == null)
        {
            ControlsText = GameObject.Find("ControlsText").GetComponent<TextMeshProUGUI>();
        }

        ControlsUI.SetActive(false);
        StartGame = false;

        ControlsText.text = SetControlsText();

        ObjectiveUI.SetActive(false);
        ChangeScene = true;

        ReturnScene = false;

        TrackID = 1;

        ButtonTrackOption[0].Select();

        MainMenuIdle.PlayAudio();
    }

    // Update is called once per frame
    void Update()
    {
        ControlsText.text = SetControlsText();
        var val = SelectControl.ReadValue<Vector3>();

        if(val.x == 1 || val.y == 1)
        {
            condition = 1;
        }
        else if(val.x == -1 || val.y == -1)
        {
            condition = -1;
        }

        switch(condition)
        {
            case 1:
                control = Controls.Standard;
                break;
            case -1:
                control = Controls.Inverted;
                break;
        }

    }

    private void SelectToGoToNextScreen(InputAction.CallbackContext callback)
    {
        if (StartGame)
        {

            //GoIntoTrackScene(SetTrackNum(TrackID));
            SceneManager.LoadScene(TrackID);
           
        }
        else if (ChangeScene)
        {
            MenuUI.SetActive(false);
            ControlsUI.SetActive(true);
            ChangeScene= false;
        }
        else 
        {
            if(!ReturnScene)
            {
                ControlsUI.SetActive(false);
                ObjectiveUI.SetActive(true);
                ReturnScene = true;
                StartGame = true;
            }          
            
        }
        
    }

    private void ReturnToPreviousScreen(InputAction.CallbackContext callback)
    {
        if(ReturnScene)
        {
            ControlsUI.SetActive(true);
            ObjectiveUI.SetActive(false);
            ReturnScene = false;
            StartGame = false;
        }
    }
    
    private string SetControlsText()
    {
        string output = $"Press D-pad to select control:\r\n{control.ToString()}";

        return output;
    }


    

    
}