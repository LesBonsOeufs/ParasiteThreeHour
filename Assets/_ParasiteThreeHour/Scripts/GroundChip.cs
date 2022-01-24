///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 12:20
///-----------------------------------------------------------------
using Com.LesBonsOeufs.ParasiteThreeHour;
using Com.LesBonsOeufs.ParasiteThreeHour.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public class GroundChip : MonoBehaviour
    {
        public static List<Transform> List { get; private set; } = new List<Transform>();

        private void Awake()
        {
            List.Add(transform);
        }

        public static Transform CreateChip(Vector2 position, Vector2 scale, Color color, Transform parent)
        {
            Transform lGroundChipTransform = Instantiate(GameManager.Instance.GroundChipObject, parent).transform;

            lGroundChipTransform.position = position;
            lGroundChipTransform.localScale = scale;
            lGroundChipTransform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

            return lGroundChipTransform;
        }

        public void Break()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            List.Remove(transform);
        }
    }
}
