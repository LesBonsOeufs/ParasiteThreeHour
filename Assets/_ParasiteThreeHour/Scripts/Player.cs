///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:00
///-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void PlayerEventHandler(Player sender);
    public delegate void PlayerWithFloatEventHandler(Player sender, float value);

    public class Player : MonoBehaviour
    {
        private const string ANIMATOR_DIGGING_PARAMETER_NAME = "isDigging";
        private const string ANIMATOR_SCREAMING_PARAMETER_NAME = "isScreaming";
        private const string ANIMATOR_INIT_SCREAM_PARAMETER_NAME = "initScream";

        [SerializeField] private string groundBlockTag = "GroundBlock";
        [SerializeField] private float diggingSpeed = 2f;
        [SerializeField] private float initScreamDuration = 0.16f;
        [SerializeField] private float screamDuration = 1f;
        [SerializeField] private float screamCooldown = 5f;
        [SerializeField] private float stunDuration = 2.5f;
        [SerializeField] private Color stunColor = new Color(0.223f, 0.223f, 0.223f);
        [SerializeField] private int nStunColorLoops = 3;

        [Header("Sounds")]
        [SerializeField] private List<AudioClip> diggingClips;
        [SerializeField] private AudioClip screamingClip;
        [SerializeField] private List<AudioClip> stunnedClips;

        public static bool PoisonGround = false;

        private Animator animator;
        private ParticleSystem digParticles;
        private KeyController controller;
        private AudioSource audioSource;

        public int Digit { get; private set; }

        private UnityAction DoAction;

        private Transform pivot;
        private float stunCounter;
        private float screamCounter;
        private float screamInitCounter;
        private float _screamCooldownCounter;

        public float ScreamCooldownCounter
        {
            get
            {
                return _screamCooldownCounter;
            }
            set
            {
                _screamCooldownCounter = value;
                OnScreamCounterUpdate?.Invoke(this, Mathf.Clamp01(1f - _screamCooldownCounter / screamDuration));
            }
        }

        public static event PlayerWithFloatEventHandler OnDigDown;
        public static event PlayerWithFloatEventHandler OnScream;
        public static event PlayerWithFloatEventHandler OnScreamCounterUpdate;
        public static event PlayerEventHandler OnHitChip;
        public static event PlayerEventHandler OnStunned;
        public static event PlayerEventHandler OnDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            digParticles = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            pivot = transform.parent;
            SetModeNormal();
        }

        private void KeyController_OnScream(KeyController sender)
        {
            if (ScreamCooldownCounter <= 0f)
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

        public void SetDigit(int digit)
        {
            Digit = digit;
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
            //Check Death
            if (Camera.main.WorldToScreenPoint(pivot.position).y / Camera.main.pixelHeight > 1f)
                Die();

            if (ScreamCooldownCounter > 0f)
                ScreamCooldownCounter -= Time.deltaTime;

            if (controller.isDigging)
            {
                pivot.Translate(diggingSpeed * Time.deltaTime * -Vector3.up);
                OnDigDown?.Invoke(this, diggingSpeed * Time.deltaTime);
            }

            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, controller.isDigging);
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
            Destroy(pivot.gameObject);
        }

        private void SetModeInitScream()
        {
            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, false);
            animator.SetTrigger(ANIMATOR_INIT_SCREAM_PARAMETER_NAME);

            DoAction = DoActionInitScream;
            screamInitCounter = initScreamDuration;
            screamCounter = screamDuration;
            ScreamCooldownCounter = screamCooldown;
        }

        private void DoActionInitScream()
        {
            if (screamInitCounter > 0f)
                screamInitCounter -= Time.deltaTime;

            if (screamInitCounter <= 0f)
                SetModeScream();
        }

        private void SetModeScream()
        {
            animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, true);

            DoAction = DoActionScream;

            OnScream?.Invoke(this, screamDuration);
            ScreamCooldownCounter = screamCooldown;

            audioSource.clip = screamingClip;
            audioSource.Play();
        }

        private void DoActionScream()
        {
            if (screamCounter > 0f)
                screamCounter -= Time.deltaTime;

            if (screamCounter <= 0f)
            {
                animator.SetBool(ANIMATOR_SCREAMING_PARAMETER_NAME, false);
                SetModeNormal();
            }
        }

        public void SetModeStunned()
        {
            animator.SetBool(ANIMATOR_DIGGING_PARAMETER_NAME, false);

            DoAction = DoActionStunned;
            OnStunned?.Invoke(this);
            stunCounter = stunDuration;

            SpriteRenderer lSpriteRenderer = GetComponent<SpriteRenderer>();
            Color lInitColor = lSpriteRenderer.color;
            float lIndividualTweenDuration = stunDuration / (nStunColorLoops * 2f);

            int lRandomIndex = Mathf.RoundToInt(Random.value * (stunnedClips.Count - 1));
            audioSource.clip = stunnedClips[lRandomIndex];
            audioSource.Play();

            DOTween.Sequence(lSpriteRenderer)
                .Append(lSpriteRenderer.DOColor(stunColor, lIndividualTweenDuration))
                .Append(lSpriteRenderer.DOColor(lInitColor, lIndividualTweenDuration)).SetLoops(nStunColorLoops);
        }

        private void DoActionStunned()
        {
            if (stunCounter > 0f)
                stunCounter -= Time.deltaTime;

            if (stunCounter <= 0f)
                SetModeNormal();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(groundBlockTag))
            {
                int lRandomIndex = Mathf.RoundToInt(Random.value * (diggingClips.Count - 1));
                audioSource.clip = diggingClips[lRandomIndex];
                audioSource.Play();

                digParticles.startColor = collision.GetComponent<SpriteRenderer>().color;
                digParticles.Emit(10);
                OnHitChip?.Invoke(this);
                collision.GetComponent<GroundBlock>().Break();
            }
        }

        private void OnDestroy()
        {
            controller.OnScream -= KeyController_OnScream;
            controller = null;
        }
    }
}
