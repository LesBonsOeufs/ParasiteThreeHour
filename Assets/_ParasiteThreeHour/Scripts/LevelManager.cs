///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 10:41
///-----------------------------------------------------------------
using Com.LesBonsOeufs.ParasiteThreeHour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void LevelManagerEventHandler(LevelManager sender);

    public class LevelManager : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color groundLayer1Color = Color.white;
        [SerializeField] private Color groundLayer2Color = Color.grey;
        [SerializeField] private Color groundLayer3Color = Color.black;

        [Header("Fields")]
        [SerializeField] private int levelLength = 30;
        [SerializeField] private int levelHeight = 30;
        [SerializeField] private int nbHorizontalSquares = 12;
        [SerializeField] private int nbLayer1Squares = 30;
        [SerializeField] private int nbLayer2Squares = 15;
        [SerializeField] private int nbLayer3Squares = 7;
        [SerializeField] private int chipPerSquare = 6;

        public int LevelLength => levelLength;
        public int LevelHeight => levelHeight;

        public static LevelManager Instance { get; private set; }

        public Vector3 ChipScale { get; private set; }

        private Vector3 worldOriginPosition;

        public event LevelManagerEventHandler OnLevelLoadEnd;

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
            worldOriginPosition = GameManager.Instance.WorldOriginPoint.position;
        }

        public void InitLevel()
        {
            Debug.Log("Start!");

            Vector3 lInitPosition = worldOriginPosition;
            Vector3 lPosition = new Vector3();
            ChipScale = new Vector3((float)levelLength / nbHorizontalSquares,
                                         (float)levelHeight / (nbLayer1Squares + nbLayer2Squares + nbLayer3Squares) / chipPerSquare, 1f);
            //Vector3 lChipScale = lScale;
            //lChipScale.y /= chipPerSquare;

            for (int i = 0; i < nbLayer1Squares; i++)
            {
                for (int j = 0; j < nbHorizontalSquares; j++)
                {
                    for (int k = 0; k < chipPerSquare; k++)
                    {
                        lPosition.y = lInitPosition.y - ChipScale.y * ((i * chipPerSquare) + k);
                        lPosition.x = lInitPosition.x + ChipScale.x * j;
                        
                        GroundChip.CreateChip(lPosition, ChipScale, groundLayer1Color);
                    }
                }
            }

            lInitPosition.y -= nbLayer1Squares * ChipScale.y * chipPerSquare;

            for (int i = 0; i < nbLayer2Squares; i++)
            {
                for (int j = 0; j < nbHorizontalSquares; j++)
                {
                    for (int k = 0; k < chipPerSquare; k++)
                    {
                        lPosition.y = lInitPosition.y - ChipScale.y * ((i * chipPerSquare) + k);
                        lPosition.x = lInitPosition.x + ChipScale.x * j;

                        GroundChip.CreateChip(lPosition, ChipScale, groundLayer2Color);
                    }
                }
            }

            lInitPosition.y -= nbLayer2Squares * ChipScale.y * chipPerSquare;

            for (int i = 0; i < nbLayer3Squares; i++)
            {
                for (int j = 0; j < nbHorizontalSquares; j++)
                {
                    for (int k = 0; k < chipPerSquare; k++)
                    {
                        lPosition.y = lInitPosition.y - ChipScale.y * ((i * chipPerSquare) + k);
                        lPosition.x = lInitPosition.x + ChipScale.x * j;

                        GroundChip.CreateChip(lPosition, ChipScale, groundLayer3Color);
                    }
                }
            }

            OnLevelLoadEnd?.Invoke(this);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
