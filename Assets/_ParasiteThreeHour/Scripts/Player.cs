///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:00
///-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

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
        [SerializeField] private float screamCooldown = 5f;
        [SerializeField] private float stunDuration = 2.5f;
        [SerializeField] private Color stunColor = new Color(0.223f, 0.223f, 0.223f);
        [SerializeField] private int nStunColorLoops = 3;

        public static bool PoisonGround = false;

        private Animator animator;
        private ParticleSystem digParticles;
        private KeyController controller;

        private UnityAction DoAction;

        private float counter = 0f;

        public static event PlayerEventHandler OnDigDown;
        public static event PlayerScreamEventHandler OnScream;
        public static event PlayerEventHandler OnHitChip;
        public static event PlayerEventHandler OnStunned;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            digParticles = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            SetModeNormal();
        }

        private void KeyController_OnScream(KeyController sender)
        {
            if (counter <= 0f)
                SetModeInitScream();
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
            if (counter > 0f)
                counter -= Time.deltaTime;

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
            counter = initScreamDuration;
        }

        private void DoActionInitScream()
        {
            if (counter <= 0f)
                SetModeScream();
        }

        private void SetModeScream()
        {
            animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, true);

            DoAction = DoActionScream;

            OnScream?.Invoke(this, screamDuration);
            counter = screamDuration;
        }

        private void DoActionScream()
        {
            if (counter <= 0f)
            {
                counter = screamCooldown;
                animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, false);

                SetModeNormal();
            }
        }

        public void SetModeStunned()
        {
            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, false);

            DoAction = DoActionStunned;
            OnStunned?.Invoke(this);
            counter = stunDuration;

            SpriteRenderer lSpriteRenderer = GetComponent<SpriteRenderer>();
            Color lInitColor = lSpriteRenderer.color;
            float lIndividualTweenDuration = stunDuration / (nStunColorLoops * 2f);

            DOTween.Sequence(lSpriteRenderer)
                .Append(lSpriteRenderer.DOColor(stunColor, lIndividualTweenDuration))
                .Append(lSpriteRenderer.DOColor(lInitColor, lIndividualTweenDuration)).SetLoops(nStunColorLoops);
        }

        private void DoActionStunned()
        {
            if (counter <= 0f)
                SetModeNormal();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(groundChipTag))
            {
                digParticles.startColor = collision.GetComponent<SpriteRenderer>().color;
                digParticles.Emit(10);
                OnHitChip?.Invoke(this);
                collision.GetComponent<GroundChip>().Break();
            }
        }

        private void OnDestroy()
        {
            controller.OnScream -= KeyController_OnScream;
            controller = null;
        }
    }
}
