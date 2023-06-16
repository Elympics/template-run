using UnityEngine;
using TMPro;
using System;

public class CurrentEventDisplayer : MonoBehaviour
{
    [SerializeField] private string eventLabelString = "Event has ended! See you at the next one!";
    [SerializeField] private TextMeshProUGUI eventLabel;
    [SerializeField] private TextMeshProUGUI remainingTimeText;

    public DateTime TimeTo { get; set; }

    private void Update()
    {
        if (TimeTo == default)
            return;

        var remainingTime = TimeTo - DateTime.Now;

        if (remainingTime > TimeSpan.Zero)
        {
            remainingTimeText.text = $"{Mathf.FloorToInt((float)remainingTime.TotalHours):00} : {remainingTime.Minutes:00} : {remainingTime.Seconds:00}";
        }
        else
        {
            remainingTimeText.text = string.Empty;
            eventLabel.text = eventLabelString;
        }
    }
}
