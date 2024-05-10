using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//script no longer in use
public class ItemRespawn : MonoBehaviour
{
    [SerializeField]
    private PowerUp[] ItemBoxes;

    // Start is called before the first frame update
    void Start()
    {
        if(ItemBoxes.Length == 0)
        {
            ItemBoxes = GetComponentsInChildren<PowerUp>(); 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        RespawnItem();
    }

    private void RespawnItem()
    {
        var inactiveItem = ItemBoxes.FirstOrDefault(x => !x.gameObject.activeSelf);
        if(inactiveItem != null && !inactiveItem.isActiveAndEnabled)
        {
            StartCoroutine(RespawnTimer(5, inactiveItem));
        }
    }

    IEnumerator RespawnTimer(int timer, PowerUp itemBox)
    {
        yield return new WaitForSeconds(timer);
        itemBox.gameObject.SetActive(true);
        //itemBox.ResetPowerUp();
    }
}
