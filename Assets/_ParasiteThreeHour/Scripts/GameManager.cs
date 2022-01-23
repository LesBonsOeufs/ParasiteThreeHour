///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 10:22
///-----------------------------------------------------------------
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform worldOriginPoint = null;
        [SerializeField] private GameObject groundChipPivotObject = null;
        [SerializeField] private GameObject playerPivotObject = null;
        [SerializeField] private float playerChipsNbFromLevelExtremity = 3f;
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

        public GameObject GroundChipObject => groundChipPivotObject;
        public Transform WorldOriginPoint => worldOriginPoint;

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
            LevelManager.Instance.InitLevel();

            Camera.main.GetComponent<CameraMotor>().Setup();

            InitPlayers();
        }

        private void InitPlayers()
        {
            float lXShiftFromLevelExtremity = LevelManager.Instance.ChipScale.x * 0.5f + 
                                              LevelManager.Instance.ChipScale.x * playerChipsNbFromLevelExtremity;

            Transform lPlayerTransform = Instantiate(playerPivotObject).transform;
            Player lPlayerScript = lPlayerTransform.GetChild(0).GetComponent<Player>();
            //lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player1Color;
            lPlayerTransform.position = WorldOriginPoint.position + new Vector3(lXShiftFromLevelExtremity, playerYShift, 0f);

            player1Controller = new KeyController();
            player1Controller.SetKeys(player1Dig, player1Scream);
            lPlayerScript.SetController(player1Controller);
            lPlayerScript.SetRuntimeAnimatorController(player1Animations);

            lPlayerTransform = Instantiate(playerPivotObject).transform;
            lPlayerScript = lPlayerTransform.GetChild(0).GetComponent<Player>();
            //lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player2Color;
            lPlayerTransform.position = WorldOriginPoint.position 
                                        + new Vector3(LevelManager.Instance.LevelLength - lXShiftFromLevelExtremity, playerYShift, 0f);

            player2Controller = new KeyController();
            player2Controller.SetKeys(player2Dig, player2Scream);
            lPlayerScript.SetController(player2Controller);
            lPlayerScript.SetRuntimeAnimatorController(player2Animations);
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
