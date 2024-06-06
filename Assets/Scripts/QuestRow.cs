using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRow : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questGiver;

    public Button trackingButton;

    public bool isActive;
    public bool isTracking;

    public Quest thisQuest;

    private void Start()
    {
        trackingButton.onClick.AddListener(()=>
        {
            if(isActive)
            {
                if(isTracking)
                {
                    isTracking = false;
                    trackingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Track quest";
                    QuestManager.Instance.UnTrackQuest(thisQuest);
                }
                else
                {
                    isTracking = true;
                    trackingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Untrack quest";
                    QuestManager.Instance.TrackQuest(thisQuest);
                }
            }
        });
    }
}
