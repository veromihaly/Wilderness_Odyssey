using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Quest
{
    [Header("Bools")]
    public bool accepted;
    public bool declined;
    public bool initialDialogCompleted;
    public bool isCompleted;

    public bool hasNoRequirements;

    [Header("Quest Info")]
    public QuestInfo info;
}
