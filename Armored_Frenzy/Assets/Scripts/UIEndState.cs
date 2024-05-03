using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndState : MonoBehaviour
{
    [SerializeField]
    private Button[] ButtonOptions;

    [SerializeField]
    private GameObject EndingUI;

    private bool SetSelection;

    // Start is called before the first frame update
    void Start()
    {
        SetSelection = false;

        for (int i = 0; i < ButtonOptions.Length; i++)
        {
            int buttonNum = i;
            ButtonOptions[i].onClick.AddListener(() => ButtonResponce(buttonNum));
        }

        ButtonOptions[0].Select();
    }

    // Update is called once per frame
    void Update()
    {
        SetEndingStatus();
    }

    private void SetEndingStatus()
    {
        switch (UIPlayer.state)
        {
            case PlayerState.Win:
            case PlayerState.Lose:
                if(!SetSelection)
                {
                    ButtonOptions[0].Select();
                    SetSelection= true;
                }
                EndingUI.SetActive(true);
                break;
            default:
                EndingUI.SetActive(false);
                break;
        }
    }

    private void ButtonResponce(int button)
    {
        switch (button)
        {
            case 0:
                //Replay again
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case 1:
                //Go back to menu track selection
                SceneManager.LoadScene(0);

                break;
        }
    }

    
}
