using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucy.UI
{
    public sealed class SpaceHUDView : MonoBehaviour
    {
        #region Inspector

        [Header("References Space HUD View")]
        public TMP_Text speedText;
        public TMP_Text healthText;
        public TMP_Text distanceText;
        public TMP_Text coinsText;

        public Image healthFill;
        public Image speedFill;

        [Header("Settings Space HUD View")]
        [Min(0f)] public float maxSpeed = 100f;

        [Header("Debug")]
        public float speed;
        public float health = 1f;
        public float closestDistance;
        public int coins;
        public int maxCoins = 5;

        #endregion

        #region Unity messages

        void OnValidate()
        {
            Refresh();
        }

        #endregion

        #region Public methods

        public void SetSpeed(float value)
        {
            speed = Mathf.Max(0f, value);
            RefreshSpeed();
        }

        public void SetHealth(float normalizedHealth)
        {
            health = Mathf.Clamp01(normalizedHealth);
            RefreshHealth();
        }

        public void SetClosestDistance(float value)
        {
            closestDistance = Mathf.Max(0f, value);
            RefreshDistance();
        }

        public void SetCoins(int value, int maximum)
        {
            coins = Mathf.Max(0, value);
            maxCoins = Mathf.Max(0, maximum);
            RefreshCoins();
        }

        public void Refresh()
        {
            RefreshSpeed();
            RefreshHealth();
            RefreshDistance();
            RefreshCoins();
        }

        #endregion

        #region Private methods

        void RefreshSpeed()
        {
            if (speedText != null)
                speedText.text = $"{speed:0} m/s";

            if (speedFill != null)
                speedFill.fillAmount = maxSpeed > 0f ? Mathf.Clamp01(speed / maxSpeed) : 0f;
        }

        void RefreshHealth()
        {
            if (healthText != null)
                healthText.text = $"{health * 100f:0}%";

            if (healthFill != null)
                healthFill.fillAmount = health;
        }

        void RefreshDistance()
        {
            if (distanceText != null)
                distanceText.text = $"{closestDistance:0} m";
        }

        void RefreshCoins()
        {
            if (coinsText != null)
                coinsText.text = $"{coins} / {maxCoins}";
        }

        #endregion
    }
}
