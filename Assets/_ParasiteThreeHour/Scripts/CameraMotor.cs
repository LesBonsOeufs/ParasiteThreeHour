///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:05
///-----------------------------------------------------------------
using Com.LesBonsOeufs.ParasiteThreeHour.Managers;
using UnityEngine;
using DG.Tweening;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class CameraMotor : MonoBehaviour
    {
        [SerializeField] private float yOffsetFromPlayer = -1f;
        [SerializeField] private float zoom = 6f;

        [Header("Camera Movement")]
        [SerializeField] private float screamShakeStrength = 0.55f;
        [SerializeField] private float screamShakeDuration = 0.4f;
        [SerializeField] private float stunShakeStrength = 1.3f;
        [SerializeField] private float stunShakeDuration = 0.6f;
        [SerializeField] private float adjustmentDuration = 0.4f;
        [SerializeField] private float smoothFollowDuration = 0.1f;

        public void Setup()
        {
            Player.OnDigDown += Player_OnDigDown;
            Player.OnScream += Player_OnScream;
            Player.OnStunned += Player_OnStunned;

            Camera lCamera = GetComponent<Camera>();
            float lHorizontalCameraSize = (LevelManager.Instance.LevelLength - zoom) * 0.5f;

            lCamera.orthographicSize = lHorizontalCameraSize / lCamera.aspect;
            transform.position = GameManager.Instance.WorldOriginPoint.position + 
                                 new Vector3(LevelManager.Instance.LevelLength * 0.5f, yOffsetFromPlayer, transform.position.z);
        }

        private void Player_OnDigDown (Player sender)
        {
            float lPlayerYPos = sender.transform.position.y;
            Vector3 lNewPosition = new Vector3(transform.position.x, lPlayerYPos + yOffsetFromPlayer, transform.position.z);

            if (transform.position.y > lPlayerYPos + yOffsetFromPlayer)
                transform.DOMove(lNewPosition, smoothFollowDuration);
        }

        private void Player_OnScream (Player sender, float duration)
        {
            DOTween.Kill(transform);
            transform.DOShakePosition(screamShakeDuration, screamShakeStrength).OnComplete(AdjustPosition);
        }

        private void Player_OnStunned (Player sender)
        {
            DOTween.Kill(transform);
            transform.DOShakePosition(stunShakeDuration, stunShakeStrength).OnComplete(AdjustPosition);
        }

        private void AdjustPosition()
        {
            DOTween.Kill(transform);
            transform.DOMove(new Vector3(GameManager.Instance.WorldOriginPoint.position.x + 
                                 LevelManager.Instance.LevelLength * 0.5f, transform.position.y, transform.position.z),
                                 adjustmentDuration);
        }

        private void OnDestroy()
        {
            Player.OnDigDown -= Player_OnDigDown;
            Player.OnScream -= Player_OnScream;
            Player.OnStunned -= Player_OnStunned;
        }
    }
}
