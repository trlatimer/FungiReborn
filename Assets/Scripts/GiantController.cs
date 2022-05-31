using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GiantController : MonoBehaviour, IDamageable
{
    private readonly int AttackHash = Animator.StringToHash("Throw");
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int WalkHash = Animator.StringToHash("Walk");
    private readonly int DieHash = Animator.StringToHash("Dying");
    private readonly int ReviveHash = Animator.StringToHash("Standing");

    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;

    [Header("Stats")]
    public float maxHealth = 100;
    public float currHealth; 
    public float reviveCooldown;
    public float healthMultiplier;
    public List<GameObject> armorItems;

    [Header("Attacking")]
    public float attackDistance;
    public float maxAttackCooldown;
    public float reviveWaitTime;
    public GameObject rockPrefab;
    public Transform hand;
    public float power;

    [Header("Sound")]
    public AudioSource giantAudio;
    public AudioClip deathSound;
    public AudioClip reviveSound;

    private bool dead = false;
    private bool reviving = false;
    public GiantStates currentState;
    private float distanceToPlayer;
    private float timeSinceDeath = float.MaxValue;
    private float currentAttackCooldown;
    private float timeSinceAttack = float.MaxValue;
    private float timeSinceRevive = float.MaxValue;
    private int currArmorIndex = 0;

    public AnimationClip walkAnim;
    public AnimationClip idleAnim;
    public AnimationClip attackAnim;
    public AnimationClip deathAnim;
    public AnimationClip reviveAnim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currHealth = maxHealth;
        currentState = GiantStates.Idle;
        currentAttackCooldown = UnityEngine.Random.Range(0, maxAttackCooldown);
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        timeSinceAttack += Time.deltaTime;
        timeSinceDeath += Time.deltaTime;
        switch (currentState)
        {
            case GiantStates.Idle:
                Idle();
                break;
            case GiantStates.Chasing:
                ChasePlayer();
                break;
            case GiantStates.Attacking:
                Attack();
                break;
            case GiantStates.Dead:
                Die();
                break;
            case GiantStates.Reviving:
                Revive();
                break;
        }
    }

    private void Idle()
    {
        agent.isStopped = true;
        if (distanceToPlayer <= attackDistance)
            currentState = GiantStates.Attacking;
        else
            currentState = GiantStates.Chasing;
    }

    private void Attack()
    {          
        if (timeSinceAttack >= currentAttackCooldown && distanceToPlayer < attackDistance)
        {
            animator.Play(AttackHash);
            currentAttackCooldown = UnityEngine.Random.Range(0, maxAttackCooldown);
            timeSinceAttack = 0;
        }
        else if (timeSinceAttack > animator.GetCurrentAnimatorStateInfo(0).length && distanceToPlayer > attackDistance)
        {
            currentState = GiantStates.Idle;
        }  
    }

    private void ChasePlayer()
    {
        animator.Play(WalkHash);
        agent.isStopped = false;
        agent.SetDestination(player.position);
        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true;
            currentState = GiantStates.Attacking;
        }           
    }

    private void Die()
    {
        currentState = GiantStates.Dead;
        if (!dead)
        {
            animator.Play(DieHash);
            agent.isStopped = true;
            dead = true;
            timeSinceDeath = 0;
            giantAudio.PlayOneShot(deathSound);
        }

        if (timeSinceDeath >= reviveCooldown)
        {
            currentState = GiantStates.Reviving;
            dead = false;
        }               
    }

    private void Revive()
    {
        timeSinceRevive += Time.deltaTime;
        if (!reviving)
        {
            //animator.SetBool(IsRevivingHash, true);
            animator.Play(ReviveHash);
            armorItems[currArmorIndex].SetActive(true);
            currArmorIndex++;
            maxHealth *= healthMultiplier;
            currHealth = maxHealth;
            reviving = true;
            timeSinceRevive = 0;
            giantAudio.PlayOneShot(reviveSound);
        }
        if (timeSinceRevive >= reviveWaitTime)
        {
            currentState = GiantStates.Idle;
            reviving = false;
        }
    }

    public void TakeDamage(float damage)
    {
        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        if (currHealth <= 0)
            currentState = GiantStates.Dead;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<IDamageable>().TakeDamage(100);
    }

    public void ThrowRock()
    {
        var rock = Instantiate(rockPrefab, hand.position, Quaternion.identity);
        rock.GetComponent<Rigidbody>().velocity = (player.position - rock.transform.position).normalized * power;
    }
}

public enum GiantStates
{
    Chasing,
    Attacking,
    Dead,
    Reviving,
    Idle
}
