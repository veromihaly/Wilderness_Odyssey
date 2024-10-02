using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
 
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
 
 
 
    private void Awake()
    {
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
 
    }
 
 
    public void OnBeginDrag(PointerEventData eventData)
    {

        canvasGroup.alpha = .6f;
        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(transform.root);
        itemBeingDragged = gameObject;
 
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed)  and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta;
 
    }
 
 
 
    public void OnEndDrag(PointerEventData eventData)
    {
        var tempItemReference = itemBeingDragged;
 
        itemBeingDragged = null;
 
        if (transform.parent == startParent || transform.parent == transform.root)
        {
            //Hide the icon of the item at this point
            tempItemReference.SetActive(false);

            AlertDialogManager dialogManager = FindObjectOfType<AlertDialogManager>();            

            dialogManager.ShowDialog("Do you want to drop this item?", (response)=> {
                if(response) // if it is true
                {
                    DropItemIntoTheWorld(tempItemReference);
                }
                else // if not
                {
                    transform.position = startPosition;
                    transform.SetParent(startParent);

                    tempItemReference.SetActive(true);
                }

            });
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void DropItemIntoTheWorld(GameObject tempItemReference)
    {
        //Get clean name
        string cleanName = tempItemReference.name.Split(new string[] {"(Clone)"}, StringSplitOptions.None)[0];

        //Instantiate item
        GameObject item = Instantiate(Resources.Load<GameObject>(cleanName + "_Model"));

        item.transform.position = Vector3.zero;
        var dropSpawnPosition = PlayerState.Instance.playerBody.transform.Find("DropSpawn").transform.position;
        item.transform.localPosition = new Vector3(dropSpawnPosition.x, dropSpawnPosition.y, dropSpawnPosition.z);

        //Set instantiated item to be the child of [Items] object
        var itemsObject = FindObjectOfType<EnviromentManager>().gameObject.transform.Find("[Resources]");
        item.transform.SetParent(itemsObject.transform);

        //Delete item from inventory
        DestroyImmediate(tempItemReference.gameObject);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
}