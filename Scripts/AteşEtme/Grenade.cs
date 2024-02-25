using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float upForce = 0.01f;
    [SerializeField] private float explosionForce;

    [SerializeField] private GameObject explosionEffect;

    private static bool isExplosed;



    private void Start()
    {
        isExplosed = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        isExplosed = true;
        if (collision != null && isExplosed)
        {
            Explosion();
            SoundManager.PlaySound(SoundManager.Sound.Bomb);
            Destroy(gameObject,.5f);

        }
    }
    void Explosion()
    {

        Vector3 explosionPos = transform.position;
        GameObject tempEffect = Instantiate(explosionEffect, explosionPos, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(explosionPos, range);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
           
            if (hit != null && rb)
            {
                if (hit.gameObject.TryGetComponent(out EnemyController enemy))
                {
                    GameManager.instance.CreateDamageIFace(enemy, 100);
                }
                if (hit.gameObject.TryGetComponent(out Barrel barrel))
                {
                    GameManager.instance.CreateDamageIFace(barrel, 100);

                }
                StartCoroutine(GameManager.instance.CameraTitre(0.1f, .5f));
                rb.AddExplosionForce(explosionForce, explosionPos, range, upForce, ForceMode.Impulse);
                GameAssets._i.PlayParticleSystemInGameObjects(tempEffect.transform);

            }

        }
        isExplosed = false;


    }

    // Bunu Game Managerda kullan ve diðer kullanýlan yerlerde ise GameManagere eriþtirrrrr.
   

}
