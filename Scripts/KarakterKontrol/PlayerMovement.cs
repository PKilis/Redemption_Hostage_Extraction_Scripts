using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public GameManager gameManager;
    public CharacterController characterController;

    [Header("Fiziksel Ýslemler")]
    public float hiz = 6f;
    public float gravity = -9.81f; // -49,05 daha gerçekçi olmasýný saðlýyor.
    public float jumpHeight = 1.2f;
    public float hizlanmaMiktari = 1.5f;
    public float maxHiz = 13.5f;
    private float guncelHiz;
    public Vector3 crouchScale;
    public Vector3 normalScale;

    [Space]

    [Header("Zemin Kontrol Ýslemleri")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;


    [Space]

    [Header("UI")]
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyBarText;
    [SerializeField] private float myEnergyBar;
    public float maxEnergyBar;

    int layerMask;

    M4A4 m4a4Script;

    private void Start()
    {
        m4a4Script = GetComponent<M4A4>();
        layerMask = 1 << 9;
        layerMask = ~layerMask;
        normalScale = gameManager.transform.localScale;
        
    }

    void Update()
    {
        // Karakter objesinin altýnda oluþan küre bir çarpýþma algýlandýðýnda isGrounded = true olacaktýr.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, layerMask);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && move.magnitude > 0)
        {
            m4a4Script.anim.SetBool("Run", true);

            // Hýzý arttýr ve maksimum hýzý kontrol et
            guncelHiz += hizlanmaMiktari * Time.deltaTime * 2f;

            //guncelHiz ile maxHiz kýyaslanýp en düþük deðeri döndürecektir.
            guncelHiz = Mathf.Min(guncelHiz, maxHiz);
           
            // Karakteri ileri doðru hareket ettir
            characterController.Move(move * guncelHiz * Time.deltaTime);

            // Koþma tuþu basýldýkca enerjiyi azalt
            EnergyControl(guncelHiz, false, 1);

            #region KELES
          //  Keles._i.anim.SetBool("Kosma", true);
            #endregion
        }
       else if(!Input.GetKey(KeyCode.LeftShift) && myEnergyBar < maxEnergyBar)
        {
            Debug.Log("Else kýsmý hala çalýþýyor");
            m4a4Script.anim.SetBool("Run", false);
            #region KELES
            //Keles._i.anim.SetBool("Kosma", false);
            #endregion
            StartCoroutine(EnergyIncrease(1f));
            guncelHiz = hiz;
        } 
        if (Input.GetButtonDown("Jump") && isGrounded && myEnergyBar > 30)
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            EnergyControl(30, true, 30);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            characterController.height = 1f;

        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            characterController.height = 1.4f;

        }

        characterController.Move(move * hiz * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }
    public void EnergyControl(float energyCount, bool isJumping, int minValue)
    {
        if (myEnergyBar > minValue)
        {
            if (isJumping)
            {
                myEnergyBar -= energyCount;
                energyBar.fillAmount = myEnergyBar / maxEnergyBar;
                energyBarText.text = "ENERGY %" + Mathf.RoundToInt(myEnergyBar).ToString();

            }
            else
            {
                myEnergyBar -= energyCount * Time.deltaTime;
                energyBar.fillAmount = myEnergyBar / maxEnergyBar;
                if (myEnergyBar <= 0)
                {
                    energyBar.fillAmount = 0f;
                }
                energyBarText.text = "ENERGY %" + Mathf.RoundToInt(myEnergyBar).ToString();

            }

        }

    }
    public IEnumerator EnergyIncrease(float duration)
    {
       
        yield return new WaitForSeconds(duration);
            float tempAcceleration = 2;
            myEnergyBar += tempAcceleration/100f ;
            //myEnergyBar = energyCount / 100f;
            if (myEnergyBar > maxEnergyBar)
            {
                myEnergyBar = maxEnergyBar;
            }
            energyBar.fillAmount = myEnergyBar / maxEnergyBar;
            energyBarText.text = "ENERGY %" + Mathf.RoundToInt(myEnergyBar).ToString();

        

    }

}
