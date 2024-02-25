using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class M4A4 : MonoBehaviour, IDamageable
{
    public static M4A4 _i;

    [Header("Silah Ýslemleri")]
    private bool atesEdebilirMi;
    public bool flashlightOpen;
    float iceridenAtesEtmeSiklik; // fire rate
    public float disaridanAtesEtmeSiklik;
    public float menzil;
    public int sarjorKapasitesi;
    public int toplamMermiSayisi;
    public int silahtakiMermiSayisi;
    public int grenadeCount;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject grenadeThrowPoint;
    RaycastHit hit;

    [Space]

    [Header("Efektler")]
    [SerializeField] private GameObject atesEfekti;
    [SerializeField] private GameObject dumanEfekti;
    [SerializeField] private ParticleSystem[] kanParticle;
    [SerializeField] private ParticleSystem fireParticle;

    [Space]

    [Header("UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthBarText;
    [SerializeField] private TextMeshProUGUI grenadeCountText;
    [SerializeField] private TextMeshProUGUI silahtakiMermiSayisiText;
    [SerializeField] private TextMeshProUGUI toplamMermiSayisiText;


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
    private static int layerMask;

    bool waitTheReloadSound = true;
    bool isItDead;

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
        isItDead = false;
        anim.Play("NewTake");

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
        if (!isItDead)
        {

            if (Input.GetKey(KeyCode.Mouse0) && waitTheReloadSound)
            {
                if (atesEdebilirMi && Time.time > iceridenAtesEtmeSiklik && silahtakiMermiSayisi != 0)
                {
                    iceridenAtesEtmeSiklik = Time.time + disaridanAtesEtmeSiklik;
                    AtesEt();
                    silahtakiMermiSayisi--;
                    StartCoroutine(GameManager.instance.CameraTitre(titremeSure, patlamaBuyukluk));
                 
                }
                if (silahtakiMermiSayisi == 0)
                {
                    Debug.Log("Mermi Bitti");
                    SoundManager.PlaySound(SoundManager.Sound.PlayerEmptyGun);

                }

                Update_UI();



            }
            if (Input.GetKeyDown(KeyCode.R) && waitTheReloadSound)
            {
                if (silahtakiMermiSayisi < sarjorKapasitesi)
                {
                    waitTheReloadSound = false;
                    StartCoroutine(Reload());

                }

            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (grenadeCount != 0)
                {
                    anim.Play("NewThrowGrenade");
                    StartCoroutine(ThrowGrenade());

                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(benimCam.transform.position, benimCam.transform.forward , out hit, menzil))
                {                                    
                    if (hit.collider.TryGetComponent(out HostageController hostage))
                    {
                        Debug.Log("trygete girdi ");
                        IInteractable interactable = hostage.GetComponent<IInteractable>();
                        if (interactable == null) return;
                        interactable.Interact();
                        SoundManager.PlaySound(SoundManager.Sound.HostageSelect);

                    }
                }
            }
        }
    }

   
    public void AtesEt()
    {
        SoundManager.PlaySound(SoundManager.Sound.M4A4Shoot);
        anim.Play("NewShoot");
        //StartCoroutine(CameraTitre(titremeSure, patlamaBuyukluk));
        AtesEtmeEfekt();

        CreateRay();
    }

    public void CreateRay()
    {

        if (Physics.Raycast(benimCam.transform.position, benimCam.transform.forward, out hit, menzil, GameManager.instance.layerMask))
        {
            GameObject temp = Object_Pooling.Instance.GetPooledObject();


            if (hit.collider.TryGetComponent(out EnemyController enemy))
            {
                GameManager.instance.CreateDamageIFace(enemy, 40); // -- IDamageable damageable = enemy.GetComponent<IDamageable>(); -- if (damageable == null) return; -- damageable.Damage(40); */

                ParticleSystem tempParticle = Instantiate(kanParticle[Random.Range(0, 2)], hit.point, Quaternion.LookRotation(hit.normal));
                tempParticle.transform.SetParent(hit.collider.gameObject.transform);
            }
            if (hit.collider.TryGetComponent(out Barrel barrel))
            {
                GameManager.instance.CreateDamageIFace(barrel, 40);
                ParticleSystem tempParticle = Instantiate(fireParticle, hit.point, Quaternion.LookRotation(hit.normal));
                tempParticle.transform.SetParent(hit.collider.gameObject.transform);
            }
            else if (!hit.collider.gameObject.CompareTag("Hostage"))
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

            anim.Play("NewReload");
            SoundManager.PlaySound(SoundManager.Sound.M4A4Reload);

            //anim.Play("Keles_MermiDegistir");            
            yield return new WaitForSeconds(2.5f);
            waitTheReloadSound = true;
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
    IEnumerator ThrowGrenade()
    {
        yield return new WaitForSeconds(.86f);
        Vector3 throwY = new Vector3(grenadeThrowPoint.transform.position.x, grenadeThrowPoint.transform.position.y + 1.5f, grenadeThrowPoint.transform.position.z);
        GameObject obje = Instantiate(bomb, throwY, grenadeThrowPoint.transform.rotation);
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

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0) health = 0; isItDead = true;
        Update_UI();
    }
    internal void Update_UI()
    {
        silahtakiMermiSayisiText.text = silahtakiMermiSayisi.ToString();
        toplamMermiSayisiText.text = toplamMermiSayisi.ToString();

        grenadeCountText.text = grenadeCount.ToString();

        healthBarText.text = "HEALTH  %" + health.ToString();
        healthBar.fillAmount = health / maxHealth;
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(benimCam.transform.position, benimCam.transform.forward * 8, Color.red);
    }
}
