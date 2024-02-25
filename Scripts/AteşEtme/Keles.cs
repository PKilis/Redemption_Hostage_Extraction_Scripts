using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Keles : MonoBehaviour, IDamageable
{
    public static Keles _i;

    [Header("Silah Ýslemleri")]
    public bool atesEdebilirMi;
    public bool flashlightOpen;
    float iceridenAtesEtmeSiklik; // fire rate
    public float disaridanAtesEtmeSiklik;
    public float menzil;
    public int sarjorKapasitesi;
    public int toplamMermiSayisi;
    public int silahtakiMermiSayisi;
    public int grenadeCount;
    public GameObject bomb;
    public GameObject bombPoint;
    RaycastHit hit;

    [Space]

    [Header("Efektler")]
    public GameObject atesEfekti;
    public GameObject flashlight;
    public GameObject dumanEfekti;


    [Space]

    [Header("UI")]
    public Image healthBar;
    public TextMeshProUGUI healthBarText;
    public TextMeshProUGUI grenadeCountText;
    public TextMeshProUGUI silahtakiMermiSayisiText;
    public TextMeshProUGUI toplamMermiSayisiText;


    [Space]

    [Header("Kamera Ve Shake")]
    public Camera benimCam;
    public float titremeSure;
    public float patlamaBuyukluk;

    public Animator anim;

    //Partikül efektleri çocuk obje içerisinde olan objelere eriþmek.
    Transform atesEfektiTransform;
    public float health;
    public static float maxHealth = 100f;

    private void Awake()
    {
        if (_i != null && _i != this)
        {
            Destroy(this);
        }
        else
        {
            _i = this;
        }
        SoundManager.Initilaize();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {


        //        StartCoroutine(GameManager.instance.CreateCollectableObjects());
        toplamMermiSayisi = toplamMermiSayisi - sarjorKapasitesi;
        silahtakiMermiSayisi = sarjorKapasitesi;
        MermiDegistirmeIslemleri("NormalYaz");

        atesEdebilirMi = true;
        flashlightOpen = true;
        atesEfektiTransform = atesEfekti.transform;
        PlayerPrefs.GetInt("AK47", toplamMermiSayisi);
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (atesEdebilirMi && Time.time > iceridenAtesEtmeSiklik && silahtakiMermiSayisi != 0)
            {
                iceridenAtesEtmeSiklik = Time.time + disaridanAtesEtmeSiklik;
                AtesEt();
                silahtakiMermiSayisi--;

            }
            if (silahtakiMermiSayisi == 0)
            {
                Debug.Log("Mermi Bitti");
                SoundManager.PlaySound(SoundManager.Sound.PlayerEmptyGun);

            }

            Update_UI();



        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightOpen = !flashlightOpen;
            flashlight.SetActive(flashlightOpen);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (silahtakiMermiSayisi < sarjorKapasitesi)
            {

                StartCoroutine(Reload());

            }

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grenadeCount != 0)
            {
                anim.Play("ThrowGrenade");
                StartCoroutine(ThrowGrenade());
                
            }
        }

    }

    IEnumerator ThrowGrenade()
    {
        yield return new WaitForSeconds(.86f);
        GameObject obje = Instantiate(bomb, bombPoint.transform.position, bombPoint.transform.rotation);
        Vector3 acimiz = Quaternion.AngleAxis(90, benimCam.transform.forward) * transform.forward;
        Rigidbody rg = obje.GetComponent<Rigidbody>();
        rg.AddForce(acimiz * 500);
        grenadeCount--;
        if (grenadeCount <= 0)
        {
            grenadeCount = 0;
        }
        Update_UI();
    }
    public void AtesEt()
    {
        //atesEtmeSesi.Play();
        SoundManager.PlaySound(SoundManager.Sound.KelesShoot);
        anim.Play("AtesEt");
        StartCoroutine(CameraTitre(titremeSure, patlamaBuyukluk));
        AtesEtmeEfekt();

        CreateRay();
    }

    public void CreateRay()
    {

        if (Physics.Raycast(benimCam.transform.position, benimCam.transform.forward, out hit, menzil, GameManager.instance.layerMask))
        {
            GameObject temp = Object_Pooling.Instance.GetPooledObject();

            if (temp != null)
            {
                temp.transform.position = hit.point;
                temp.transform.rotation = Quaternion.LookRotation(hit.normal);
                temp.SetActive(true);
                StartCoroutine(MakeSetActiveTrue(temp));  
            }
        }

    }

    void AtesEtmeEfekt()
    {
        GameAssets._i.PlayParticleSystemInGameObjects(atesEfektiTransform);
    }
    IEnumerator Reload()
    {


        if (silahtakiMermiSayisi < sarjorKapasitesi && toplamMermiSayisi != 0)
        {
            anim.Play("Keles_MermiDegistir");
            yield return new WaitForSeconds(1.35f);
            SoundManager.PlaySound(SoundManager.Sound.KelesReload);

            if (silahtakiMermiSayisi != 0)
            {
                MermiDegistirmeIslemleri("MermiVar");
            }
            else
            {
                MermiDegistirmeIslemleri("MermiYok");
            }


        }
    }
    void MermiDegistirmeIslemleri(string tur)
    {


        switch (tur)
        {
            case "MermiVar":
                if (toplamMermiSayisi <= sarjorKapasitesi)
                {
                    int distaKalanMermi = toplamMermiSayisi + silahtakiMermiSayisi;
                    if (distaKalanMermi > sarjorKapasitesi)
                    {
                        silahtakiMermiSayisi = sarjorKapasitesi;
                        toplamMermiSayisi = distaKalanMermi - sarjorKapasitesi;
                        PlayerPrefs.SetInt("AK47", toplamMermiSayisi);

                    }
                    else
                    {
                        silahtakiMermiSayisi += toplamMermiSayisi;
                        toplamMermiSayisi = 0;
                        PlayerPrefs.SetInt("AK47", toplamMermiSayisi);

                    }
                }
                else
                {
                    toplamMermiSayisi -= sarjorKapasitesi - silahtakiMermiSayisi;
                    silahtakiMermiSayisi = sarjorKapasitesi;
                    PlayerPrefs.SetInt("AK47", toplamMermiSayisi);

                }
                Update_UI();
                break;

            case "MermiYok":
                if (toplamMermiSayisi <= sarjorKapasitesi)
                {
                    silahtakiMermiSayisi = toplamMermiSayisi;
                    toplamMermiSayisi = 0;

                }
                else
                {
                    toplamMermiSayisi -= sarjorKapasitesi;
                    PlayerPrefs.SetInt("AK47", toplamMermiSayisi);
                    silahtakiMermiSayisi = sarjorKapasitesi;


                }
                Update_UI();
                break;

            case "NormalYaz":
                Update_UI();
                break;

        }

    }

    IEnumerator MakeSetActiveTrue(GameObject temp)
    {
        yield return new WaitForSeconds(3f);
        temp.SetActive(false);

    }
    internal void Update_UI()
    {
        silahtakiMermiSayisiText.text = silahtakiMermiSayisi.ToString();
        toplamMermiSayisiText.text = toplamMermiSayisi.ToString();

        grenadeCountText.text = grenadeCount.ToString();

        healthBarText.text = "HEALTH  %" + health.ToString();
        healthBar.fillAmount = health / maxHealth;
    }

    IEnumerator CameraTitre(float titremeSuresi, float magnitude)
    {
        Vector3 orjinalPozisyon = benimCam.transform.localPosition;

        float gecenSure = 0.0f;
        while (gecenSure < titremeSuresi)
        {
            float x = Random.Range(-1, 1) * magnitude;

            benimCam.transform.localPosition = new Vector3(x, orjinalPozisyon.y, orjinalPozisyon.z);
            gecenSure += Time.deltaTime;
            yield return null;
        }
        benimCam.transform.localPosition = orjinalPozisyon;
    }

    public void Damage(int damageAmount)
    {
        IDamageable damageable = GetComponent<IDamageable>();
        if (damageable == null) return;
        health -= damageAmount;
        if (health <= 0) health = 0;
        Update_UI();
    }

}
