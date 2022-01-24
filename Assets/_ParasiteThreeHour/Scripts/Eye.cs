///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 23/01/2022 18:01
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class Eye : MonoBehaviour
    {
        private const string ANIMATOR_OPENED_PARAMETER_NAME = "opened";
        private const string ANIMATOR_ATTACK_PARAMETER_NAME = "attack";

        private Animator animator;

        public bool Open
        {
            get
            {
                return animator.GetBool(ANIMATOR_OPENED_PARAMETER_NAME);
            }

            set
            {
                animator.SetBool(ANIMATOR_OPENED_PARAMETER_NAME, value);
            }
        }

        public bool Attack
        {
            get
            {
                return animator.GetBool(ANIMATOR_ATTACK_PARAMETER_NAME);
            }

            set
            {
                animator.SetBool(ANIMATOR_ATTACK_PARAMETER_NAME, value);
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}
