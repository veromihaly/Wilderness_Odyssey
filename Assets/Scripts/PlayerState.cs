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

    // ---- Player Hydration ---- //
    public float currentHydrationPercent;
    public float maxHydrationPercent;

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
    }

    void Update()
    {
        //testing the bars
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentHealth -= 10;
            currentCalories -= 100;
            currentHydrationPercent -= 10;
        }
    }
}
