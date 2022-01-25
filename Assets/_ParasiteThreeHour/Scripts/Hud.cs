///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 25/01/2022 02:23
///-----------------------------------------------------------------
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class Hud : MonoBehaviour
    {
        [SerializeField] private Color player1WinBackgroundColor = Color.black;
        [SerializeField] private Color player1WinTextColor = Color.black;
        [SerializeField] private Color player2WinBackgroundColor = Color.black;
        [SerializeField] private Color player2WinTextColor = Color.black;
        [SerializeField] private Image winBackground = null;
        [SerializeField] private Text winText = null;
        [SerializeField] private float winAnimDuration = 1f;

        public static Hud Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Color lTransparentColor = new Color(0f, 0f, 0f, 0f);
            winBackground.color = lTransparentColor;
            winText.color = lTransparentColor;

            Player.OnDeath += Player_OnDeath;
        }

        private void Player_OnDeath(Player sender, int digit)
        {
            if (digit == 2)
            {
                winBackground.DOColor(player1WinBackgroundColor, winAnimDuration);
                winText.DOColor(player1WinTextColor, winAnimDuration);

                winText.text = winText.text.Replace('#', '1');
            }
            else if (digit == 1)
            {
                winBackground.DOColor(player2WinBackgroundColor, winAnimDuration);
                winText.DOColor(player2WinTextColor, winAnimDuration);

                winText.text = winText.text.Replace('#', '2');
            }
        }

        private void OnDestroy()
        {
            Player.OnDeath -= Player_OnDeath;
            Instance = null;
        }
    }
}
