///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 10:41
///-----------------------------------------------------------------
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour.Managers {
    public class LevelManager : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color groundLayer1Color = Color.white;
        [SerializeField] private Color groundLayer2Color = Color.grey;
        [SerializeField] private Color groundLayer3Color = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color groundFinalLayerColor = Color.black;

        [Header("Fields")]
        [SerializeField] private int _levelLength = 30;
        [SerializeField] private int nHorizontalBlocks = 7;
        [SerializeField] private int nLayer1Lines = 3;
        [SerializeField] private int nLayer2Lines = 7;
        [SerializeField] private int nLayer3Lines = 4;
        [SerializeField] private int chipPerBlock = 6;
        [SerializeField] private float blockYScale = 1f;
        
        [Header("In-Game")]
        [SerializeField] private float playerMinYPos = 1f;

        private int currentLine;

        public static LevelManager Instance { get; private set; }

        public List<float> ChippedGroundXPositions { get; private set; }
        public int LevelLength => _levelLength;
        //public int LevelHeight => _levelHeight;
        public Vector3 BlockScale { get; private set; }
        public Vector3 ChipScale { get; private set; }
        public Transform Level { get; private set; }

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

            Level = new GameObject("Level").transform;
            Level.position = GameManager.Instance.WorldOriginPoint.position;

            float lBlockSideLength = (float)LevelLength / nHorizontalBlocks;

            if (blockYScale == 0f)
                blockYScale = lBlockSideLength;

            BlockScale = new Vector3(lBlockSideLength, blockYScale, 1f);
            ChipScale = new Vector3(lBlockSideLength, blockYScale / chipPerBlock, 1f);
        }

        private void Start()
        {
            Player.OnDigDown += Player_OnDigDown;
            Player.OnHitChip += Player_OnHitChip;
        }

        private void Player_OnDigDown(Player sender, float speed)
        {
            //float lPlayerYPos = sender.transform.position.y;
            //Vector3 lNewPosition = new Vector3(transform.position.x, lPlayerYPos + yOffsetFromPlayer, transform.position.z);

            if (sender.transform.position.y < playerMinYPos)
                Level.Translate(Vector3.up * speed);
        }
        
        private void Player_OnHitChip(Player sender)
        {
            UpdateLevel();
        }

        public void InitLevel(List<float> lChippedGroundXPositions)
        {
            Debug.Log("Start!");

            currentLine = 0;
            ChippedGroundXPositions = lChippedGroundXPositions;

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            Vector3 lLevelPosition = Level.position;
            Vector3 lPosition = new Vector3();
            Color lLayerColor;

            while (Camera.main.WorldToScreenPoint(new Vector3(0f, lLevelPosition.y - BlockScale.y * currentLine
                   + GroundBlock.ADDED_Y_FOR_OUT_OF_SCREEN, 0f)).y / Camera.main.pixelHeight > 0f)
            {
                if (currentLine < nLayer1Lines)
                    lLayerColor = groundLayer1Color;
                else if (currentLine < nLayer1Lines + nLayer2Lines)
                    lLayerColor = groundLayer2Color;
                else if (currentLine < nLayer1Lines + nLayer2Lines + nLayer3Lines)
                    lLayerColor = groundLayer3Color;
                else
                    lLayerColor = groundFinalLayerColor;

                for (int i = 0; i < nHorizontalBlocks; i++)
                {
                    lPosition.x = lLevelPosition.x + BlockScale.x * i;

                    if (ChippedGroundXPositions.Contains(lPosition.x))
                    {
                        for (int j = 0; j < chipPerBlock; j++)
                        {
                            lPosition.y = lLevelPosition.y - ChipScale.y * ((currentLine * chipPerBlock) + j);
                            GroundBlock.CreateBlock(lPosition, ChipScale, lLayerColor, Level);
                        }
                    }
                    else
                    {
                        lPosition.y = lLevelPosition.y - BlockScale.y * currentLine;
                        GroundBlock.CreateBlock(lPosition, BlockScale, lLayerColor, Level);
                    }
                }

                currentLine++;
            }
        }

        private void OnDestroy()
        {
            Player.OnDigDown -= Player_OnDigDown;
            Player.OnHitChip -= Player_OnHitChip;
            Instance = null;
        }
    }
}