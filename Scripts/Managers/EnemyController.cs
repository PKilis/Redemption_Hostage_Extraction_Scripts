using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{

    // ÖLME ANÝMASYONUNU DEÐÝÞTÝRRRRR !! 

    NavMeshAgent agent;
    Animator anim;
    Transform player;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] private GameObject Gun;
    [SerializeField] private Transform GunParent;
    [SerializeField] private Transform atesEfektiTransform;

    //Patroling
    int walkPointNumber;
    public float walkPointRange;
    public Transform[] WalkPoints;


    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool _isItDead;

    //States
    [SerializeField] private float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    Vector3 distanceToWalkPoint;
    string previousAnimationName;
    Quaternion targetRotation;

    private int _health;
    
    
    // PROPERTIES
    public int Health 
    {
        get { return _health; } 
        set { _health = value; }
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

    }

    private void Start()
    {
        _health = 100;
        walkPointNumber = 0;
        StartCoroutine(SetParent());

    }

    private void Update()
    {
       
        if (!_isItDead)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


            if (!playerInSightRange && !playerInAttackRange) Patroll();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }

    }

    private void Patroll()
    {
        agent.isStopped = false;
        PlayAnimation("Walk");
        //PlayAnimation("WalkWithGun");

        agent.SetDestination(WalkPoints[walkPointNumber].position);
        float distanceToDestination = Vector3.Distance(transform.position, WalkPoints[walkPointNumber].position);

        if (distanceToDestination < 0.3f)
        {
            StartCoroutine(LookAround());
            walkPointNumber++;
            if (walkPointNumber > WalkPoints.Length - 1)
            {
                walkPointNumber = 0;

            }
        }

    }

    IEnumerator LookAround()
    {
        PlayAnimation("Idle");
        targetRotation = Quaternion.LookRotation(WalkPoints[walkPointNumber].transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

        yield return new WaitForSeconds(5f);

    }

    IEnumerator SetParent()
    {
        Gun.transform.SetParent(GunParent);
        yield return new WaitForSeconds(3);

    }
    private void ChasePlayer()
    {
        agent.isStopped = false;

        targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

        PlayAnimation("Chase");
        //PlayAnimation("RunWithGun");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.isStopped = true;
        agent.SetDestination(player.position);

        //transform.LookAt(player);
        targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);


        if (!alreadyAttacked)
        {
            GameAssets._i.PlayParticleSystemInGameObjects(atesEfektiTransform);
            SoundManager.PlaySound(SoundManager.Sound.KelesShoot);
            agent.isStopped = true;
            PlayAnimation("Attack");

            GameManager.instance.CreateDamageIFace(player.GetComponent<M4A4>(), 5);

            // Attack Code Here!!
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);


        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);


            Handles.Label(Vector3.Lerp(transform.position, player.position, .5f), Vector3.Distance(transform.position, player.position).ToString());

        }
    }

    // Önceki animasyonu durdur ve yeni animasyonu baþlat
    public void PlayAnimation(string animationName)
    {
        // Eðer animator bileþeni yoksa veya animationName null veya boþsa, iþlemi sonlandýr
        if (anim == null || string.IsNullOrEmpty(animationName))
            return;

        // Eðer önceki animasyon adý boþ deðilse, önceki animasyonu durdur
        if (!string.IsNullOrEmpty(previousAnimationName))
        {
            anim.SetBool(previousAnimationName, false);
        }

        // Yeni animasyonu baþlat
        anim.SetBool(animationName, true);

        // Önceki animasyon adýný güncelle
        previousAnimationName = animationName;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if (_health <= 0)
        {
            PlayAnimation("Die");
            _isItDead = true;
            Destroy(gameObject, 2f);
        }        
    }
}


