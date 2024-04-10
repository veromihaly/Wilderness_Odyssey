using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class EquipableItem : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !InventorySystem.Instance.isOpen && !CraftingSystem.Instance.isOpen) //Left Mouse Button
        {   
            StartCoroutine(SwingSoundDelay());
            animator.SetTrigger("hit");
        }
    }

    public void GetHit()
    {
       GameObject selectedTree = SelectionManager.Instance.selectedTree;
        if(selectedTree != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.chopTreeSound);
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        } 
    }

    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
    }
}
