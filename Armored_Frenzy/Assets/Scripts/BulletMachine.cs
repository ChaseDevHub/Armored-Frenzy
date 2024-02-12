using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public enum BulletMachineSide { left, right };
public class BulletMachine : MonoBehaviour
{
    public BulletMachineSide side;
    public GameObject Prefab;
    public List<Bullet> Bullets;
    public int MaxBullets;

    private void Start()
    {
        switch (side)
        {
            case BulletMachineSide.left:
            case BulletMachineSide.right:
                for (int i = 0; i < MaxBullets; i++)
                {
                    GameObject temp = Instantiate(Prefab);
                    Bullets.Add(temp.GetComponent<Bullet>());
                }

                foreach (var v in Bullets)
                {
                    v.gameObject.SetActive(false);
                }

                break;
        
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
        yield return new WaitForSeconds(0.5f);
        inactiveBullet.FireFrom(transform);
        inactiveBullet.gameObject.SetActive(true);
        StopAllCoroutines();
    }


   
}
