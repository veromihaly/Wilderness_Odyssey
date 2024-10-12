using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance {get; set;}

    // ---- Player Health ---- //
    public float currentHealth;
    public float maxHealth;

    // ---- Player Calories ---- //
    public float currentCalories;
    public float maxCalories;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;

    // ---- Player Hydration ---- //
    public float currentHydrationPercent;
    public float maxHydrationPercent;

    public bool isHydrationActive;

     private bool isDecreasingHealth = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;
        
        StartCoroutine(decreaseHydration());
    }

    IEnumerator decreaseHydration()
    {
        while (currentHydrationPercent > 0)
        {
            currentHydrationPercent -= 1;
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator decreaseHealth()
    {
        currentHealth -= 1;
        yield return new WaitForSeconds(4);
        isDecreasingHealth = false;
    }

    void Update()
    {
        distanceTravelled += Vector3.Distance(playerBody.transform.position,lastPosition);
        lastPosition = playerBody.transform.position;

        if(distanceTravelled >= 5 && currentCalories != 0)
        {
            distanceTravelled = 0;
            currentCalories -= 1;
        }
        if(currentHydrationPercent == 0 && !isDecreasingHealth || currentCalories == 0 && !isDecreasingHealth)
        {
            isDecreasingHealth = true;
            StartCoroutine(decreaseHealth());   
        }
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }

    public void setHydration(float newHydration)
    {
        currentHydrationPercent = newHydration;
    }
}
