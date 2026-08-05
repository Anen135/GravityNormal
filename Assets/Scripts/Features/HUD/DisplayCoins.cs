using UnityEngine;

public class DisplayCoins : MonoBehaviour
{
    public Counter coinCounter;
    public TMPro.TMP_Text coinText;

    void OnEnable()
    {
        if (coinCounter == null || coinText == null) Debug.LogWarning("Coin Counter is not assigned.", this);
        else coinCounter.OnCountChanged.AddListener(UpdateCoinDisplay);
    }

    void OnDisable()
    {
        if (coinCounter == null || coinText == null) Debug.LogWarning("Coin Counter or Coin Text is not assigned.", this);
        else coinCounter.OnCountChanged.RemoveListener(UpdateCoinDisplay);
    }

    void UpdateCoinDisplay(int newCoinCount)
    {
        if (coinText == null || coinCounter == null) Debug.LogWarning("Coin Text or Coin Counter is not assigned.", this);
        else coinText.text = $"Coins: {newCoinCount}/{coinCounter.TargetCount}";
    }
}