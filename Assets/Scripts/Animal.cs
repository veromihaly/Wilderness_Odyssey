using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{

    public string animalName;
    public bool playerInRange;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int damage;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip rabbitHitAndScream;
    [SerializeField] AudioClip rabbitHitAndDie;
    [SerializeField] AudioClip catfishHitAndScream;
    [SerializeField] AudioClip catfishHitAndDie;

    private  Animator animator;

    public bool isDead;

    public bool playerIsFollowed = false;

    public bool playerBeingAttacked = false;
    public bool swingWait = false;

    [SerializeField] ParticleSystem bloodSplashParticles;
    public GameObject bloodPuddle;

    enum AnimalType
    {
        Rabbit,
        Catfish
    }

    enum AnimalBehaviour
    {
        Passive,
        Aggressive
    }

    [SerializeField] AnimalType thisAnimalType;
    [SerializeField] AnimalBehaviour thisAnimalBehaviour;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(playerIsFollowed && !isDead)
        {
            MoveTowardsPlayer();
            LookAtPlayer();
        }
        if(playerInRange && thisAnimalBehaviour == AnimalBehaviour.Aggressive && !isDead && !playerBeingAttacked)
        {
            animator.ResetTrigger("Following");
            playerBeingAttacked = true;
            playerIsFollowed = false;
            LookAtPlayer();
            AI_Movement.Instance.isWalking = false;
            GetComponent<AI_Movement>().enabled = false;
            animator.SetTrigger("Attack");
            StartCoroutine(AttackPlayer());
        }
    }

    public void TakeDamage(int damage)
    {
        if(!isDead && !swingWait)
        {
            swingWait = true;

            currentHealth -= damage;

            bloodSplashParticles.Play();

            if (currentHealth <= 0)
            {
                PlayDyingSound();
                animator.SetTrigger("Die");
                GetComponent<AI_Movement>().enabled = false;

                StartCoroutine(PuddleDelay());
                isDead = true;
                swingWait = false;
            }
            else
            {
                StartCoroutine(SwingDelay());
                PlayHitSound();
            }
        }
    }

    IEnumerator SwingDelay()
    {
        yield return new WaitForSeconds(1f);
        swingWait = false;
    }

    IEnumerator PuddleDelay()
    {
        yield return new WaitForSeconds(1f);
        bloodPuddle.SetActive(true);
    }

    private void PlayDyingSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndDie);
                break;
            case AnimalType.Catfish:
                soundChannel.PlayOneShot(catfishHitAndDie);
                break;
            default:
                break;
            //
        }
    }

    private void PlayHitSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndScream);
                break;
            case AnimalType.Catfish:
                soundChannel.PlayOneShot(catfishHitAndScream);
                break;
            default:
                break;
            //
        }
    }

    IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(0.3f);
        animator.ResetTrigger("Attack");
    }

    public void LookAtPlayer()
    {
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
 
        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0,yRotation,0);
 
    }
    IEnumerator AttackPlayer()
    {
        float healthBeforeAttack = PlayerState.Instance.currentHealth;
        yield return new WaitForSeconds(1.3f);
        playerBeingAttacked = false;
        if(playerInRange)
        {    
            PlayerState.Instance.setHealth(healthBeforeAttack - damage);
        }
        else
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Following");
            playerIsFollowed = true;
            StartCoroutine(AttackReset());
            if(!playerBeingAttacked && playerIsFollowed)
            {
                StartCoroutine(FollowPlayer());
            }
            animator.ResetTrigger("Wait");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    IEnumerator FollowPlayer()
    {
        yield return new WaitForSeconds(4f);
        playerIsFollowed = false;
        animator.SetTrigger("Wait");
        animator.ResetTrigger("Following");
        if(!isDead)
        {
            GetComponent<AI_Movement>().enabled = true;
            AI_Movement.Instance.waitCounter = 0;
        }
    }
    private void MoveTowardsPlayer()
    {
        Vector3 direction = (PlayerState.Instance.playerBody.transform.position - transform.position).normalized;
        transform.position += direction * AI_Movement.Instance.moveSpeed * Time.deltaTime * 2;
    }
}
