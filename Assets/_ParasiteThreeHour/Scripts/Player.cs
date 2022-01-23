///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:00
///-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.Events;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void PlayerEventHandler(Player sender);
    public delegate void PlayerScreamEventHandler(Player sender, float duration);

    public class Player : MonoBehaviour
    {
        private const string ANIMATOR_DIGGING_PARAMETER_NAME = "isDigging";
        private const string ANIMATOR_SCREAMING_PARAMETER_NAME = "isScreaming";
        private const string ANIMATOR_INIT_SCREAM_PARAMETER_NAME = "initScream";

        [SerializeField] private string groundChipTag = "GroundChip";
        [SerializeField] private float diggingSpeed = 2f;
        [SerializeField] private float initScreamDuration = 0.1f;
        [SerializeField] private float screamDuration = 1f;
        [SerializeField] private float unsafeGroundDurationOnScream = 0.7f;
        [SerializeField] private float screamCooldown = 5f;
        [SerializeField] private float stunDurationOnHitUnsafeGround = 1.5f;

        public static bool PoisonGround = false;

        private Animator animator;
        private KeyController controller;
        private float initScreamCounter = 0f;
        private float screamCounter = 0f;
        private UnityAction DoAction;

        public static event PlayerEventHandler OnDigDown;
        public static event PlayerScreamEventHandler OnScream;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            SetModeNormal();
        }

        private void KeyController_OnScream(KeyController sender)
        {
            if (screamCounter <= 0f)
            {
                Debug.Log("BURG");

                SetModeInitScream();
            }
        }

        public void SetController(KeyController keyController)
        {
            if (controller != null)
                controller.OnScream -= KeyController_OnScream;

            controller = keyController;
            controller.OnScream += KeyController_OnScream;
        }

        public void SetRuntimeAnimatorController(RuntimeAnimatorController runtimeAnimatorController)
        {
            animator.runtimeAnimatorController = runtimeAnimatorController;
        }

        private void Update()
        {
            if (screamCounter > 0f)
                screamCounter -= Time.deltaTime;

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

            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, controller.isDigging);
        }

        private void SetModeInitScream()
        {
            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, false);
            animator.SetTrigger(ANIMATOR_INIT_SCREAM_PARAMETER_NAME);

            DoAction = DoActionInitScream;
            initScreamCounter = initScreamDuration;
        }

        private void DoActionInitScream()
        {
            initScreamCounter -= Time.deltaTime;

            if (initScreamCounter <= 0f)
                SetModeScream();
        }

        private void SetModeScream()
        {
            animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, true);

            DoAction = DoActionScream;
            OnScream?.Invoke(this, screamDuration);
            screamCounter = screamDuration;
        }

        private void DoActionScream()
        {
            if (screamCounter <= 0f)
            {
                screamCounter = screamCooldown;
                animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, false);

                SetModeNormal();
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
