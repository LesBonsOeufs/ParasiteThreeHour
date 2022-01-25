///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 10:22
///-----------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.LesBonsOeufs.ParasiteThreeHour.Managers {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform _worldOriginPoint = null;

        [Header("Objects")]
        [SerializeField] private GameObject _groundBlockPivotObject = null;
        [SerializeField] private GameObject playerPivotObject = null;
        [SerializeField] private GameObject _eyeObject = null;

        [Header("Players positioning")]
        [SerializeField] private float playerNChipsFromLevelExtremity = 3f;
        [SerializeField] private float playerYShift = 0.5f;
        //[SerializeField] private Color player1Color = Color.green;
        //[SerializeField] private Color player2Color = Color.red;

        [Header("Animators")]
        [SerializeField] private RuntimeAnimatorController player1Animations = null;
        [SerializeField] private RuntimeAnimatorController player2Animations = null;

        [Header("Controls")]
        [SerializeField] private KeyCode player1Dig = KeyCode.S;
        [SerializeField] private KeyCode player1Scream = KeyCode.M;
        [SerializeField] private KeyCode player2Dig = KeyCode.Keypad2;
        [SerializeField] private KeyCode player2Scream = KeyCode.H;

        private KeyController player1Controller;
        private KeyController player2Controller;

        public GameObject GroundBlockPivotObject => _groundBlockPivotObject;
        public GameObject EyeObject => _eyeObject;
        public Transform WorldOriginPoint => _worldOriginPoint;

        public static GameManager Instance { get; private set; }

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
            Camera.main.GetComponent<CameraMotor>().Setup();
            InitPlayers(LevelManager.Instance.Level);

            List<int> lChippedGroundPositions = new List<int>();

            float lChippedGroundXShiftFromLevelExtremity = LevelManager.Instance.ChipScale.x * playerNChipsFromLevelExtremity;
            lChippedGroundPositions.Add(Mathf.FloorToInt(WorldOriginPoint.position.x + lChippedGroundXShiftFromLevelExtremity));
            lChippedGroundPositions.Add(Mathf.FloorToInt(WorldOriginPoint.position.x + LevelManager.Instance.LevelLength - 
                                        LevelManager.Instance.BlockScale.x - lChippedGroundXShiftFromLevelExtremity));

            LevelManager.Instance.InitLevel(lChippedGroundPositions);
        }

        private void InitPlayers(Transform parent)
        {
            float lPlayerXShiftFromLevelExtremity = LevelManager.Instance.ChipScale.x * 0.5f + 
                                              LevelManager.Instance.ChipScale.x * playerNChipsFromLevelExtremity;

            Transform lPlayerTransform = Instantiate(playerPivotObject, parent).transform;
            Player lPlayerScript = lPlayerTransform.GetChild(0).GetComponent<Player>();
            //lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player1Color;
            lPlayerTransform.position = WorldOriginPoint.position + new Vector3(lPlayerXShiftFromLevelExtremity, playerYShift, 0f);

            player1Controller = new KeyController();
            player1Controller.SetKeys(player1Dig, player1Scream);
            lPlayerScript.SetController(player1Controller);
            lPlayerScript.SetRuntimeAnimatorController(player1Animations);
            lPlayerScript.SetDigit(1);

            lPlayerTransform = Instantiate(playerPivotObject, parent).transform;
            lPlayerScript = lPlayerTransform.GetChild(0).GetComponent<Player>();
            //lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player2Color;
            lPlayerTransform.position = WorldOriginPoint.position 
                                        + new Vector3(LevelManager.Instance.LevelLength - lPlayerXShiftFromLevelExtremity, playerYShift, 0f);

            player2Controller = new KeyController();
            player2Controller.SetKeys(player2Dig, player2Scream);
            lPlayerScript.SetController(player2Controller);
            lPlayerScript.SetRuntimeAnimatorController(player2Animations);
            lPlayerScript.SetDigit(2);
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
        }

        private void Update()
        {
            player1Controller.CheckInputs();
            player2Controller.CheckInputs();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
