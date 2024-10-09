using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{

    public string animalName;
    public bool playerInRange;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip rabbitHitAndScream;
    [SerializeField] AudioClip rabbitHitAndDie;
    [SerializeField] AudioClip catfishHitAndScream;
    [SerializeField] AudioClip catfishHitAndDie;

    private  Animator animator;

    public bool isDead;
    public bool swingWait = false;

    [SerializeField] ParticleSystem bloodSplashParticles;
    public GameObject bloodPuddle;

    enum AnimalType
    {
        Rabbit,
        Catfish
    }

    [SerializeField] AnimalType thisAnimalType;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
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
