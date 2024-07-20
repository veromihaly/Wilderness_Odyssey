using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class SelectionManager : MonoBehaviour
{

    public static SelectionManager Instance {get;set;}

    public bool onTarget;

    public GameObject selectedObject;
 
    public GameObject interaction_Info_UI;
    Text interaction_text;

    public Image centerDotImage;
    public Image handIcon;

    public GameObject selectedTree;
    public GameObject chopHolder;
 
    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }
 
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            NPC npc = selectionTransform.GetComponent<NPC>();
            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Talk";
                interaction_Info_UI.SetActive(true);

                if(Input.GetKeyDown(KeyCode.E) && !npc.isTalkingWithPlayer)
                {
                    npc.StartConversation();
                }

                if(DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }
            }

            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            if(choppableTree && choppableTree.playerInRange)
            {
                choppableTree.canBeChopped = true;
                selectedTree = choppableTree.gameObject;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if(selectedTree != null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }

            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);
            }

            Animal animal = selectionTransform.GetComponent<Animal>();
            if(animal && animal.playerInRange)
            {
                if(animal.isDead)
                {
                    interaction_text.text = "Loot";
                    interaction_Info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);

                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_Info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);

                    if(Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon())
                    {   
                        StartCoroutine(DealDamageTo(animal, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                    }
                }
            }

            if(!interactable && !animal)
            {
                onTarget = false;

                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
            }

            if(!npc && !interactable && !animal && !choppableTree)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
 
        }
    }

    private void Loot(Lootable lootable)
    {
        if(lootable.wasLootCalculated == false)
        {
            List<LootRecieved> recievedLoot = new List<LootRecieved>();
            foreach(LootPossibility loot in lootable.possibleLoot)
            {
                var lootAmount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax+1);
                if(lootAmount != 0)
                {
                    LootRecieved lt = new LootRecieved();
                    lt.item = loot.item;
                    lt.amount = lootAmount;

                    recievedLoot.Add(lt);
                }
            }

            lootable.finalLoot = recievedLoot;
            lootable.wasLootCalculated = true; 
        }

        //Spawning the loot on the ground
        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;

        foreach(LootRecieved lootRecieved in lootable.finalLoot)
        {
            for(int i = 0;i < lootRecieved.amount;i++)
            {
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name+"_Model"),
                new Vector3(lootSpawnPosition.x, lootSpawnPosition.y+0.2f, lootSpawnPosition.z),Quaternion.Euler(0,0,0));
            }
        }

        //Blood puddle stays on the ground
        if(lootable.GetComponent<Animal>())
        {
            lootable.GetComponent<Animal>().bloodPuddle.transform.SetParent(lootable.transform.parent);
        }

        //Destroy Looted body
        Destroy(lootable.gameObject);
    }

    IEnumerator DealDamageTo(Animal animal, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);

        animal.TakeDamage(damage);
    }

    public void DisableSelection()
    {
        handIcon.enabled = false;
        centerDotImage.enabled = false;
        interaction_Info_UI.SetActive(false);

        selectedObject = null;
    }

    public void EnableSelection()
    {
        handIcon.enabled = true;
        centerDotImage.enabled = true;
        interaction_Info_UI.SetActive(true);
    }
}