///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:05
///-----------------------------------------------------------------
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class CameraMotor : MonoBehaviour
    {
        [SerializeField] private float yOffsetFromLevel = -1.5f;
        [SerializeField] private float yOffsetFromPlayer = -1f;
        [SerializeField] private float zoom = 0.2f;

        public void Setup()
        {
            Player.OnDigDown += Player_OnDigDown;

            Camera lCamera = Camera.main;
            float lHorizontalCameraSize = (LevelManager.Instance.LevelLength - zoom) * 0.5f;

            lCamera.orthographicSize = lHorizontalCameraSize / lCamera.aspect;
            lCamera.transform.position = GameManager.Instance.WorldOriginPoint.position + 
                                         new Vector3(LevelManager.Instance.LevelLength * 0.5f, yOffsetFromPlayer, lCamera.transform.position.z);
        }

        private void Player_OnDigDown (Player sender)
        {
            float lPlayerYPos = sender.transform.position.y;

            if (transform.position.y > lPlayerYPos + yOffsetFromPlayer)
                transform.position = new Vector3(transform.position.x, lPlayerYPos + yOffsetFromPlayer, transform.position.z);
        }

        private void OnDestroy()
        {
            Player.OnDigDown -= Player_OnDigDown;
        }
    }
}
