///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 23/01/2022 15:00
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class DangerousGroundManager : MonoBehaviour
    {
        public static DangerousGroundManager Instance { get; private set; }
        public bool IsDangerous { get; private set; }

        private float dangerousCounter;

        private UnityAction DoAction;


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
        }

        private void Start()
        {
            SetModeVoid();
        }

        private void Update()
        {
            DoAction();
        }

        private void SetModeVoid()
        {
            DoAction = DoActionVoid;
        }

        private void DoActionVoid() { }

        public void SetToDangerous(float forDuration)
        {
            IsDangerous = true;
            dangerousCounter = forDuration;
            DoAction = DoActionDangerous;
        }

        private void DoActionDangerous()
        {
            dangerousCounter -= Time.deltaTime;

            if (dangerousCounter <= 0f)
            {
                IsDangerous = false;
                SetModeVoid();
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
