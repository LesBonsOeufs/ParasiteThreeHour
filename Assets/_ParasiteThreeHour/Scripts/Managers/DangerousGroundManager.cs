///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 23/01/2022 15:00
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Com.LesBonsOeufs.ParasiteThreeHour.Managers {
    public class DangerousGroundManager : MonoBehaviour
    {
        [SerializeField] private float dangerousGroundDuration = 0.7f;
        [SerializeField] private float animDurationOnStun = 1f;
        [SerializeField] private int minEyesIfDangerous = 25;
        [SerializeField] private int maxEyesIfDangerous = 35;

        public static DangerousGroundManager Instance { get; private set; }
        public bool IsDangerous { get; private set; }

        private List<Eye> eyesStock = new List<Eye>();
        private List<Eye> currentEyes = new List<Eye>();
        private float counter;

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
            Player.OnScream += Player_OnScream;
            Player.OnHitChip += Player_OnHitChip;

            InitEyesStock();
            SetModeVoid();
        }

        private void InitEyesStock()
        {
            GameObject lEyeOriginalObject = GameManager.Instance.EyeObject;
            GameObject lEyeInstance;

            for (int i = 0; i < maxEyesIfDangerous; i++)
            {
                lEyeInstance = Instantiate(lEyeOriginalObject);
                eyesStock.Add(lEyeInstance.GetComponent<Eye>());
                lEyeInstance.SetActive(false);
            }
        }

        private void Player_OnScream(Player sender, float duration)
        {
            SetToDangerous();
        }

        private void Player_OnHitChip(Player sender)
        {
            if (IsDangerous)
            {
                sender.SetModeStunned();
                EyesAttack(true);

                IsDangerous = false;

                counter = animDurationOnStun;
            }
        }

        private void Update()
        {
            DoAction();
        }

        private void SetModeVoid()
        {
            EyesAttack(false);
            EyesOpen(false);
            DoAction = DoActionVoid;
        }

        private void DoActionVoid() { }

        private void SetToDangerous()
        {
            if (counter <= 0f)
            {
                RandomEyesPositionAndQuantity();
                IsDangerous = true;
            }

            counter = dangerousGroundDuration;
            DoAction = DoActionDangerous;

            EyesOpen(true);
        }

        private void DoActionDangerous()
        {
            counter -= Time.deltaTime;

            if (counter <= 0f)
            {
                IsDangerous = false;
                SetModeVoid();
            }
        }

        private void RandomEyesPositionAndQuantity()
        {
            currentEyes.Clear();
            int lNbOfEyes = Random.Range(minEyesIfDangerous, maxEyesIfDangerous + 1);

            for (int i = 0; i < lNbOfEyes; i++)
            {
                currentEyes.Add(eyesStock[i]);
            }

            Eye lEye;
            List<Transform> lChips = GroundChip.List;
            Transform lChip;
            Vector2 lChipPivotPosition;
            int lRandomChipIndex;
            Vector2 lOriginPoint = GameManager.Instance.WorldOriginPoint.position;
            Vector2 lLevelSize = new Vector2(LevelManager.Instance.LevelLength, LevelManager.Instance.LevelHeight);
            List<Vector2Int> lBannedPositions = new List<Vector2Int>();

            for (int i = 0; i < currentEyes.Count; i++)
            {
                lEye = currentEyes[i];
                lRandomChipIndex = Random.Range(0, lChips.Count);

                lChip = lChips[lRandomChipIndex];
                lChipPivotPosition = lChip.parent.position;

                if (lBannedPositions.Contains(Vector2Int.FloorToInt(lChipPivotPosition))
                       || lChipPivotPosition.x == lOriginPoint.x || lChipPivotPosition.y == lOriginPoint.y
                       || lChipPivotPosition.x == lOriginPoint.x + lLevelSize.x || lChipPivotPosition.y == lOriginPoint.y - lLevelSize.y)
                {
                    lEye.gameObject.SetActive(false);
                    continue;
                }

                lEye.transform.position = lChip.position - Vector3.forward;
                lBannedPositions.Add(Vector2Int.FloorToInt(lChipPivotPosition));

                lEye.gameObject.SetActive(true);
            }
        }

        private void EyesOpen(bool value)
        {
            foreach (Eye lEye in currentEyes)
            {
                lEye.Open = value;
            }
        }

        public void EyesAttack(bool value)
        {
            foreach (Eye lEye in currentEyes)
            {
                lEye.Attack = value;
            }
        }

        private void OnDestroy()
        {
            Player.OnScream -= Player_OnScream;
            Player.OnHitChip -= Player_OnHitChip;

            Instance = null;
        }
    }
}
