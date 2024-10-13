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
        if(playerInRange && thisAnimalBehaviour == AnimalBehaviour.Aggressive && !isDead)
        {
            animator.SetBool("Following",false);
            playerBeingAttacked = true;
            playerIsFollowed = false;
            LookAtPlayer();
            AI_Movement.Instance.isWalking = false;
            GetComponent<AI_Movement>().enabled = false;
            animator.SetBool("Attack",true);
            StartCoroutine(AttackPlayer());
        }
    }

    IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Attack",false);
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
        yield return new WaitForSeconds(1.3f);  // Attack delay

        if (playerInRange)
        {    
            PlayerState.Instance.setHealth(healthBeforeAttack - damage);
        }
        else
        {
            // Start following the player immediately if they are out of range
            playerBeingAttacked = false;
            StartCoroutine(AttackReset());
            animator.SetBool("Following",true);
            playerIsFollowed = true;
            
            // Ensure the FollowPlayer coroutine is started when not attacking
            if (!playerBeingAttacked && playerIsFollowed)
            {
                StartCoroutine(FollowPlayer());
                animator.SetBool("Wait",false);  // Reset idle state
            }
        }
    }

    IEnumerator FollowPlayer()
    {
        yield return new WaitForSeconds(5f);
        if(!playerBeingAttacked && playerIsFollowed)
        {
            playerIsFollowed = false;
            animator.SetBool("Following",false); // Ensure following is reset
            animator.SetBool("Wait",true);       // Return to idle after following

            if (!isDead)
            {
                GetComponent<AI_Movement>().enabled = true;
                AI_Movement.Instance.waitCounter = 1;
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        var speed = AI_Movement.Instance.moveSpeed * 20;
        Vector3 direction = (PlayerState.Instance.playerBody.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
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
}
