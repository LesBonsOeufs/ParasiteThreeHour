///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 25/01/2022 02:23
///-----------------------------------------------------------------
using Com.LesBonsOeufs.ParasiteThreeHour.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class Hud : MonoBehaviour
    {
        [Header("ScreamBars")]
        [SerializeField] private Transform player1ScreamBar = default;
        [SerializeField] private Transform player2ScreamBar = default;

        [Header("Win")]
        [SerializeField] private Color player1WinBackgroundColor = Color.black;
        [SerializeField] private Color player1WinTextColor = Color.black;
        [SerializeField] private Color player2WinBackgroundColor = Color.black;
        [SerializeField] private Color player2WinTextColor = Color.black;
        [SerializeField] private Image winBackground = null;
        [SerializeField] private Text winText = null;
        [SerializeField] private Image winRestartImage = null;
        [SerializeField] private Button winRestartButton = null;
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

            winBackground.gameObject.SetActive(false);
            
            Player.OnScreamCounterUpdate += Player_OnScreamCounterUpdate;
            Player.OnDeath += Player_OnDeath;
            winRestartButton.onClick.AddListener(ResetButton_OnClick);
        }

        private void Player_OnScreamCounterUpdate(Player sender, float ratio)
        {
            if (sender.Digit == 1)
                player1ScreamBar.localScale = new Vector3(ratio, 1f, 1f);
            else if (sender.Digit == 2)
                player2ScreamBar.localScale = new Vector3(ratio, 1f, 1f);
        }

        private void Player_OnDeath(Player sender)
        {
            Color lTransparentColor = new Color(0f, 0f, 0f, 0f);
            winBackground.color = lTransparentColor;
            winRestartImage.color = lTransparentColor;
            winText.color = lTransparentColor;

            winBackground.gameObject.SetActive(true);

            if (sender.Digit == 1)
            {
                winBackground.DOColor(player2WinBackgroundColor, winAnimDuration);
                winRestartImage.DOColor(player2WinTextColor, winAnimDuration);
                winText.DOColor(player2WinTextColor, winAnimDuration);

                winText.text = winText.text.Replace('#', '2');
            }
            else if (sender.Digit == 2)
            {
                winBackground.DOColor(player1WinBackgroundColor, winAnimDuration);
                winRestartImage.DOColor(player1WinTextColor, winAnimDuration);
                winText.DOColor(player1WinTextColor, winAnimDuration);

                winText.text = winText.text.Replace('#', '1');
            }
        }

        private void ResetButton_OnClick()
        {
            winBackground.gameObject.SetActive(false);
            player1ScreamBar.localScale = Vector3.one;
            player2ScreamBar.localScale = Vector3.one;
            GameManager.Instance.Restart();
        }

        private void OnDestroy()
        {
            Player.OnScreamCounterUpdate -= Player_OnScreamCounterUpdate;
            Player.OnDeath -= Player_OnDeath;
            winRestartButton.onClick.RemoveListener(ResetButton_OnClick);
            Instance = null;
        }
    }
}
