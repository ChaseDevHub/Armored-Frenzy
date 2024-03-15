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
        //Help from Chat.gpt
        var inactiveBullet = Bullets.FirstOrDefault(x => !x.gameObject.activeSelf);
        if(inactiveBullet != null)
        {
            FireBullet(inactiveBullet);
        }
       
    }

    private void FireBullet(Bullet inactiveBullet)
    {
        inactiveBullet.FireFrom(transform);
        inactiveBullet.gameObject.SetActive(true);
    }

   
}
