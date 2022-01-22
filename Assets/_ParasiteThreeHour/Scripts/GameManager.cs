///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 10:22
///-----------------------------------------------------------------
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform worldOriginPoint;
        [SerializeField] private GameObject groundChipObject;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private float playerChipsNbFromLevelExtremity = 3f;
        [SerializeField] private float playerYShift = 0.5f;
        [SerializeField] private Color player1Color = Color.green;
        [SerializeField] private Color player2Color = Color.red;

        [Header("Controls")]
        [SerializeField] private KeyCode player1Dig = KeyCode.S;
        [SerializeField] private KeyCode player1Scream = KeyCode.M;
        [SerializeField] private KeyCode player2Dig = KeyCode.Keypad2;
        [SerializeField] private KeyCode player2Scream = KeyCode.H;

        private KeyController player1Controller;
        private KeyController player2Controller;

        public GameObject GroundChipObject => groundChipObject;
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

            Transform lPlayerTransform = Instantiate(playerObject).transform;
            lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player1Color;
            lPlayerTransform.position = WorldOriginPoint.position + new Vector3(lXShiftFromLevelExtremity, playerYShift, 0f);

            player1Controller = new KeyController();
            player1Controller.SetKeys(player1Dig, player1Scream);
            lPlayerTransform.GetChild(0).GetComponent<Player>().SetController(player1Controller);

            lPlayerTransform = Instantiate(playerObject).transform;
            lPlayerTransform.GetChild(0).GetComponent<SpriteRenderer>().color = player2Color;
            lPlayerTransform.position = WorldOriginPoint.position 
                                        + new Vector3(LevelManager.Instance.LevelLength - lXShiftFromLevelExtremity, playerYShift, 0f);

            player2Controller = new KeyController();
            player2Controller.SetKeys(player2Dig, player2Scream);
            lPlayerTransform.GetChild(0).GetComponent<Player>().SetController(player2Controller);
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
