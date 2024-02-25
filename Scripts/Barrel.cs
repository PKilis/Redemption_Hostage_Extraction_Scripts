using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{

    public float range;
    public float upForce = 0.01f;
    public float explosionForce;

    public GameObject explosionEffect;

    private int _health;

    public float shakeDuration, shakeMagnitude;
    public int Health
    {
        get { return _health; }
        set { _health = value; }

    }

    private void Start()
    {
        _health = 100;
    }
    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if (_health <= 0)
        {
            SoundManager.PlaySound(SoundManager.Sound.BarrelExplosion);
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
                    StartCoroutine(GameManager.instance.CameraTitre(shakeDuration, shakeMagnitude));
                    rb.AddExplosionForce(explosionForce, explosionPos, range, upForce, ForceMode.Impulse);
                    GameAssets._i.PlayParticleSystemInGameObjects(tempEffect.transform);
                }

            }
            Destroy(gameObject, 1f);
        }
    }
}
