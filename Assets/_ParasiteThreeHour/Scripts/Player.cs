///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:00
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void PlayerEventHandler(Player sender);

    public class Player : MonoBehaviour
    {
        [SerializeField] private string groundChipTag = "GroundChip";
        [SerializeField] private float diggingSpeed = 2f;

        private KeyController controller;

        public static event PlayerEventHandler OnDigDown;

        private void Update()
        {
            if (controller.isDigging)
            {
                transform.Translate(new Vector3(0f, -diggingSpeed * Time.deltaTime, 0f));
                OnDigDown?.Invoke(this);
            }
        }

        public void SetController(KeyController keyController)
        {
            if (controller != null)
                controller.OnScream -= KeyController_OnScream;

            controller = keyController;
            controller.OnScream += KeyController_OnScream;
        }

        private void KeyController_OnScream(KeyController sender)
        {
            Debug.Log("BURG");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("burger");

            if (collision.CompareTag(groundChipTag))
                collision.GetComponent<GroundChip>().Break();
        }

        private void OnDestroy()
        {
            controller.OnScream -= KeyController_OnScream;
            controller = null;
        }
    }
}
