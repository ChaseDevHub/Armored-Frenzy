using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjects : MonoBehaviour
{
    [SerializeField]
    private GameObject ParticleEffect;

    private float ZRotation;

    [SerializeField]
    private int Health;

    // Start is called before the first frame update
    void Start()
    {
        ParticleEffect.SetActive(false);

        int temp = Random.Range(50, 150);
        ZRotation= temp;

        this.transform.Rotate(new Vector3(0, 0, ZRotation));

        if(Health == 0)
        {
            Health = 6;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(2, 2, 0);

        if(Health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Health -= collision.gameObject.GetComponent<Bullet>().DamagePoint;

            ParticleEffect.SetActive(true);

            StartCoroutine(WaitTimer());
        }
    }

    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(0.25f);

        ParticleEffect.SetActive(false);

        StopAllCoroutines();
    }
}
