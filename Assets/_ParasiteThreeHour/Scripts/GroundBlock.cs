///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:20
///-----------------------------------------------------------------
using Com.LesBonsOeufs.ParasiteThreeHour.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour {
    public class GroundBlock : MonoBehaviour
    {
        public const float ADDED_Y_FOR_OUT_OF_SCREEN = 1f;

        public static List<Transform> List { get; private set; } = new List<Transform>();

        private Transform pivot;

        private void Awake()
        {
            Player.OnDigDown += Player_OnHitChip;
            List.Add(transform);
        }

        private void Start()
        {
            pivot = transform.parent;
        }

        public static Transform CreateBlock(Vector2 position, Vector2 scale, Color color, Transform parent)
        {
            Transform lGroundBlockTransform = Instantiate(GameManager.Instance.GroundBlockPivotObject, parent).transform;

            lGroundBlockTransform.position = position;
            lGroundBlockTransform.localScale = scale;
            lGroundBlockTransform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

            return lGroundBlockTransform;
        }

        private void Player_OnHitChip(Player sender, float s)
        {
            if (Camera.main.WorldToScreenPoint(new Vector3(0f, pivot.position.y - pivot.localScale.y - ADDED_Y_FOR_OUT_OF_SCREEN, 0f)).y 
                / Camera.main.pixelHeight > 1f)
                Destroy(pivot.gameObject);
        }

        public void Break()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Player.OnDigDown -= Player_OnHitChip;
            List.Remove(transform);
        }
    }
}
