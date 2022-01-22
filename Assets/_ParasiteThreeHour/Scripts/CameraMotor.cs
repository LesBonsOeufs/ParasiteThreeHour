///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:05
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class CameraMotor : MonoBehaviour
    {
        [SerializeField] private float yOffsetFromPlayer = -1.5f;

        private void Start()
        {
            Player.OnDigDown += Player_OnDigDown;
            LevelManager.Instance.OnLevelLoadEnd += LevelManager_OnLevelLoadEnd;
        }

        private void Player_OnDigDown (Player sender)
        {
            float lPlayerYPos = sender.transform.position.y;

            if (transform.position.y > lPlayerYPos + yOffsetFromPlayer)
                transform.position = new Vector3(transform.position.x, lPlayerYPos + yOffsetFromPlayer, transform.position.z);
        }

        private void LevelManager_OnLevelLoadEnd (LevelManager sender)
        {
            Camera lCamera = Camera.main;
            float lHorizontalCameraSize = sender.LevelLength * 0.5f;

            lCamera.orthographicSize = lHorizontalCameraSize / lCamera.aspect;
            lCamera.transform.position = GameManager.Instance.WorldOriginPoint.position + new Vector3(lHorizontalCameraSize, 0f, lCamera.transform.position.z);

            LevelManager.Instance.OnLevelLoadEnd -= LevelManager_OnLevelLoadEnd;
        }

        private void OnDestroy()
        {
            Player.OnDigDown -= Player_OnDigDown;
        }
    }
}
