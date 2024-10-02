using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    public UnityEvent OnDayPass = new UnityEvent();// Day Passed Event

    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public Season currentSeason = Season.Spring;

    private int dayPerSeason = 30;
    private int daysInCurrentSeason = 1;

    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;

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

    public int dayInGame = 1;
    public TextMeshProUGUI dayUI;

    private void Start()
    {
        UpdateUI();
    }

    public void TriggerNextDay()
    {
        dayInGame += 1;
        daysInCurrentSeason += 1;

        currentDayOfWeek = (DayOfWeek)(((int)currentDayOfWeek + 1) % 7);

        if(daysInCurrentSeason > dayPerSeason)
        {
            //Switch to next season
            daysInCurrentSeason = 1;
            currentSeason = GetNextSeason();
        }

        UpdateUI();

        OnDayPass.Invoke();
    }

    private Season GetNextSeason()
    {
        int currentSeasonIndex = (int)currentSeason; // 0 - Spring
        int nextSeasonIndex = (currentSeasonIndex + 1) % 4; // 1 - Summer
        return (Season)nextSeasonIndex;
    }

    private void UpdateUI()
    {
        dayUI.text = $"{currentDayOfWeek} {daysInCurrentSeason}.,{currentSeason}";
    }
}
