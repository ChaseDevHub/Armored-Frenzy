using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    bool StartGame;

    public static Controls control;

    float condition;


    bool ChangeScene;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.NewPlayer.Acceleration.performed += SelectToGoToNextScreen;
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
        if(StartGame)
        {
            SceneManager.LoadScene(1);
        }
        else if(ChangeScene) 
        {
            MenuUI.SetActive(false);
            ControlsUI.SetActive(true);
            ChangeScene = false;
        }
        else
        {
            ControlsUI.SetActive(false);
            ObjectiveUI.SetActive(true);
            StartGame = true;
        }
        
    }
    
    private string SetControlsText()
    {
        string output = $"Press D-pad to select control:\r\n{control.ToString()}";

        return output;
    }
}
