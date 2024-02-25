using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HostageController : MonoBehaviour, IInteractable
{
    NavMeshAgent agent;
    Transform player;
    Animator anim;
    Rigidbody rb;

    bool hasPlayerInteraction;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hasPlayerInteraction = false;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }


    void Update()
    {

        if (hasPlayerInteraction)
        {
            rb.constraints = ~RigidbodyConstraints.FreezePosition;
            agent.SetDestination(player.position);
            if (agent.velocity.magnitude > 0.000f)
            {
                anim.SetBool("SlowRun", true);
                anim.SetBool("Idle", false);
            }
            else
            {
                anim.SetBool("Idle", true);
                anim.SetBool("SlowRun", false);
            }

        }

    }

    public void Interact()
    {
        hasPlayerInteraction = !hasPlayerInteraction;
    }
}
