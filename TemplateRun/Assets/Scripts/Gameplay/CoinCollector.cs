using UnityEngine;
using TMPro;
using System;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private LayerMask coinLayer;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private float bonusTextDuration;
    private int activeBonusTexts = 0;
    private bool DoesMaskContainLayer(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
    public delegate void OnCoinPickedUp();
    private event OnCoinPickedUp coinPickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (DoesMaskContainLayer(coinLayer, collision.gameObject.layer))
        {
            scoreManager.AddToScore(difficultyManager.GetCoinValue());
            collision.gameObject.SetActive(false);
            bonusText.text = "+" + difficultyManager.GetCoinValue().ToString();
            bonusText.enabled = true;
            activeBonusTexts++;
            Invoke("TurnOffBonusText", bonusTextDuration);
            coinPickedUp?.Invoke();
            
        }
    }

    private void TurnOffBonusText()
    {
        activeBonusTexts--;
        if(activeBonusTexts <= 0) bonusText.enabled = false;
    }

    public void SubscribeToCoinPickedUp(OnCoinPickedUp action)
    {
        coinPickedUp += action;
    }
}
