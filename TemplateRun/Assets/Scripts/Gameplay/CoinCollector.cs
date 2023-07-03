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

    public event Action OnCoinPickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (DoesMaskContainLayer(coinLayer, collision.gameObject.layer))
        {
            float coinValue = difficultyManager.GetCoinValue();
            scoreManager.AddToScore(coinValue);

            collision.gameObject.SetActive(false);

            bonusText.text = $"+ {coinValue}";
            bonusText.enabled = true;
            activeBonusTexts++;
            Invoke(nameof(TurnOffBonusText), bonusTextDuration);

            OnCoinPickedUp?.Invoke();
        }
    }

    private bool DoesMaskContainLayer(LayerMask mask, int layer) => (mask & (1 << layer)) != 0;

    private void TurnOffBonusText()
    {
        activeBonusTexts--;
        if (activeBonusTexts <= 0) bonusText.enabled = false;
    }
}
