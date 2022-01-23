///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:00
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void PlayerEventHandler(Player sender);
    public delegate void PlayerScreamEventHandler(Player sender, float duration);

    public class Player : MonoBehaviour
    {
        [SerializeField] private string groundChipTag = "GroundChip";
        [SerializeField] private float diggingSpeed = 2f;
        [SerializeField] private float screamDuration = 1f;
        [SerializeField] private float unsafeGroundDurationOnScream = 0.7f;
        //[SerializeField] private float screamCooldown = 5f;
        [SerializeField] private float stunDurationOnHitUnsafeGround = 1.5f;

        public static bool PoisonGround = false;

        private KeyController controller;
        private float screamCounter = 0f;
        private UnityAction DoAction;

        public static event PlayerEventHandler OnDigDown;
        public static event PlayerScreamEventHandler OnScream;

        private void Start()
        {
            SetModeNormal();
        }

        private void Update()
        {
            DoAction();
        }
        
        private void SetModeNormal()
        {
            DoAction = DoActionNormal;
        }

        private void DoActionNormal()
        {
            if (controller.isDigging)
            {
                transform.Translate(new Vector3(0f, -diggingSpeed * Time.deltaTime, 0f));
                OnDigDown?.Invoke(this);
            }
        }

        private void SetModeScream()
        {
            DoAction = DoActionScream;
            screamCounter = screamDuration;
        }

        private void DoActionScream()
        {
            screamCounter -= Time.deltaTime;

            if (screamCounter < 0f)
                SetModeNormal();
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
            if (screamCounter <= 0f)
            {
                Debug.Log("BURG");

                OnScream?.Invoke(this, screamDuration);
                SetModeScream();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
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
