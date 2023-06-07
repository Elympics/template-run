using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SponsorsDisplayManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> sponsorSprites;
    [SerializeField] private List<Image> displaySlots;
    [SerializeField] private float sponsorDisplayTime;
    [SerializeField] private float sponsorFadeDuration;

    private int sponsorDisplayState = 0; //0 - sponsorsFadingIn, 1 - sponsordDisplayed, 2 - sponsorsFadingOut
    private float timer;
    private void RefreshSponsors()
    {
        foreach (var slot in displaySlots)
        {
            int sponsorToDisplayID = PlayerPrefs.GetInt("LastSponsorID") + 1;
            if (sponsorToDisplayID >= sponsorSprites.Count) sponsorToDisplayID = 0;
            slot.sprite = sponsorSprites[sponsorToDisplayID];
            PlayerPrefs.SetInt("LastSponsorID", sponsorToDisplayID);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        switch (sponsorDisplayState)
        {
            case 0:
                FadeIn();
                break;
            case 1:
                DisplaySponsors();
                break;
            case 2:
                FadeOut();
                break;
        }
    }

    private void FadeIn()
    {
        float newAlpha = timer / sponsorFadeDuration;
        if (newAlpha > 1)
        {
            newAlpha = 1;
            sponsorDisplayState = 1;
            timer = 0;
        }
        foreach (var slot in displaySlots) slot.color = new Color(1, 1, 1, newAlpha);
    }

    private void FadeOut()
    {
        float newAlpha = 1 - (timer / sponsorFadeDuration);
        if (newAlpha < 0)
        {
            newAlpha = 0;
            sponsorDisplayState = 0;
            timer = 0;
            RefreshSponsors();
        }
        foreach (var slot in displaySlots) slot.color = new Color(1, 1, 1, newAlpha);
    }

    private void DisplaySponsors()
    {
        if (timer > sponsorDisplayTime)
        {
            timer = 0;
            sponsorDisplayState = 2;
        }
    }

    private void OnEnable()
    {
        RefreshSponsors();
        timer = 0;
        sponsorDisplayState = 0;
        foreach (var slot in displaySlots) slot.color = new Color(1, 1, 1, 0);
    }
}
