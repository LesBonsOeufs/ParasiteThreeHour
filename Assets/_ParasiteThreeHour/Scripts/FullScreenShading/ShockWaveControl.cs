///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 23/01/2022 00:57
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Blit;
using DG.Tweening;

namespace Com.LesBonsOeufs.ParasiteThreeHour.FullScreenShading
{
    public class ShockWaveControl : MonoBehaviour
    {
        private const string FEATURE_NAME = "ShockWave";

        [SerializeField] private ForwardRendererData rendererData = null;

        List<Blit> blitRendererFeatures;

        private void Awake()
        {
            blitRendererFeatures = new List<Blit>();
            Player.OnScream += Player_OnScream;;

            foreach (ScriptableRendererFeature lFeature in rendererData.rendererFeatures)
            {
                if (lFeature.name.StartsWith(FEATURE_NAME))
                    blitRendererFeatures.Add(lFeature as Blit);
            }

            Material lShockWaveMatInstance;

            foreach (Blit lBlitRendererFeature in blitRendererFeatures)
            {
                lShockWaveMatInstance = lBlitRendererFeature.settings.blitMaterial;
                lShockWaveMatInstance.SetFloat("_timeRatio", 0f);
                lShockWaveMatInstance.SetFloat("_sizeRatio", Camera.main.aspect);

                lBlitRendererFeature.SetActive(false);
            }
        }

        private Blit GetAvailableBlitRendererFeature()
        {
            foreach (Blit lBlitRendererFeature in blitRendererFeatures)
            {
                if (!lBlitRendererFeature.isActive)
                    return lBlitRendererFeature;
            }

            return null;
        }

        private void Player_OnScream(Player sender, float duration)
        {
            ShockWaveStart(sender.transform.position, duration);
        }

        private void ShockWaveStart(Vector3 position, float duration)
        {
            Blit lBlitRendererFeature = GetAvailableBlitRendererFeature();

            if (lBlitRendererFeature == null) return;

            Material lShockWaveMatInstance = lBlitRendererFeature.settings.blitMaterial;

            lBlitRendererFeature.SetActive(true);

            lShockWaveMatInstance.SetVector("_focalPoint", Camera.main.WorldToViewportPoint(position));
            lShockWaveMatInstance.SetFloat("_speed", 1f / duration);

            StartCoroutine(ShockWaveCoroutine(lBlitRendererFeature, lShockWaveMatInstance));
        }

        private IEnumerator ShockWaveCoroutine(Blit blitRendererFeature, Material shockWaveMatInstance)
        {
            shockWaveMatInstance.SetFloat("_timeRatio", 0f);

            float lInitMagnification = shockWaveMatInstance.GetFloat("_magnification");

            while (shockWaveMatInstance.GetFloat("_timeRatio") < 1f)
            {
                yield return null;

                shockWaveMatInstance.SetFloat("_timeRatio", shockWaveMatInstance.GetFloat("_timeRatio") + Time.deltaTime);
                shockWaveMatInstance.SetFloat("_magnification", lInitMagnification - lInitMagnification
                                                                * ExpoEaseIn(shockWaveMatInstance.GetFloat("_timeRatio")));
            }

            shockWaveMatInstance.SetFloat("_magnification", lInitMagnification);
            blitRendererFeature.SetActive(false);
        }

        private float ExpoEaseIn(float ratio)
        {
            return ratio == 0 ? 0 : Mathf.Pow(2f, 10f * ratio - 10f);
        }

        private void OnDestroy()
        {
            Player.OnScream -= Player_OnScream;

            foreach (Blit lBlitRendererFeature in blitRendererFeatures)
            {
                lBlitRendererFeature.settings.blitMaterial.SetFloat("_timeRatio", 0f);
            }
        }
    }
}
