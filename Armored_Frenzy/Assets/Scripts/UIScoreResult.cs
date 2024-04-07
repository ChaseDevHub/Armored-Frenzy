using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIScoreResult : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Description;
    
    [SerializeField]
    private TextMeshProUGUI TotalScore;

    [SerializeField]
    private Canvas UIPlayerCanvas;
    
    [SerializeField]
    private Canvas UIScoreCanvas;

    private float TimeRemain;

    [SerializeField]
    private Image FadePanel;

    float FadeTime;

    // Start is called before the first frame update
    void Start()
    {
        if(Description == null)
        {
            Description = GameObject.Find("Description").GetComponent<TextMeshProUGUI>();
        }

        if(TotalScore == null)
        {
            TotalScore = GameObject.Find("TotalScore").GetComponent<TextMeshProUGUI>();
        }
    
        if(UIPlayerCanvas == null)
        {
            UIPlayerCanvas = GameObject.Find("PlayerUI").GetComponent<Canvas>();
        }

        if(UIScoreCanvas == null)
        {
            UIScoreCanvas = GameObject.Find("ScoreUI").GetComponent<Canvas>();
        }
    
        if(FadePanel == null)
        {
            FadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
        }

        UIScoreCanvas.gameObject.SetActive(false);

        TimeRemain = 1.5f;
        FadeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(UIPlayer.state == PlayerState.Win)
        {
            if (TimeRemain > 0)
            {
                TimeRemain -= Time.deltaTime;
                
                
            }
            else
            {
                if (FadeTime < 1f)
                {
                    FadeTime += 1f * Time.deltaTime;
                    FadePanel.color = new Color(0, 0, 0, FadeTime);
                }
                else if (FadeTime > 0)
                {
                    FadeTime -= 1 * Time.deltaTime;
                    FadePanel.color = new Color(0, 0, 0, FadeTime);
                }
                else
                {
                    DisplayResult();
                }
                
            }
        }
    }

    private void DisplayResult()
    {
        UIScoreCanvas.gameObject.SetActive(true);
        UIPlayerCanvas.gameObject.SetActive(false);
        Description.text = InsertDescription();
        TotalScore.text = InsertTotal();
    }

    private string InsertDescription()
    {
        string output = $"Destroyed: {ScoreData.DataInfo(1)}x\r\n\r\nRecovered: {ScoreData.DataInfo(2)}x\r\n\r\nEnergy Remained: {ScoreData.DataInfo(3)}";

        return output;
    }

    private string InsertTotal()
    {
        string output = $"Total: {ScoreData.DataInfo(4).ToString("N0")}";

        return output;
    }

    //Leaving here to change camera follow and look at 
    //https://forum.unity.com/threads/cinemachine-target-follow-an-initialize-prefab.559576/
}
