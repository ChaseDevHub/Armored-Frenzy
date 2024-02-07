using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class BulletMachine : MonoBehaviour
{
    public GameObject Prefab;
    public List<Bullet> Bullets;
    public int MaxBullets;

    private void Start()
    {
        for (int i = 0; i < MaxBullets; i++)
        {
            GameObject temp = Instantiate(Prefab);
            Bullets.Add(temp.GetComponent<Bullet>());
        }

        foreach (var v in Bullets)
        {
            v.gameObject.SetActive(false);
        }
    }

    public void Shoot()
    {
        //Bullets.Where(x => x.gameObject.activeSelf == false).First().gameObject.SetActive(true);

        //Help from Chat.gpt
        var inactiveBullet = Bullets.FirstOrDefault(x => !x.gameObject.activeSelf);
        if(inactiveBullet != null)
        {
            StartCoroutine(Wait(inactiveBullet));
        }
        else //might add a bool condition to where they can shoot first item right away instead of waiting
        {
            //nothing
        }
    }

    IEnumerator Wait(Bullet inactiveBullet)
    {
        yield return new WaitForSeconds(1);
        inactiveBullet.gameObject.SetActive(true);
        StopAllCoroutines();
    }


   
}
