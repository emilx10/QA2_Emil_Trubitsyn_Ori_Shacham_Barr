using System;
using UnityEngine;

namespace TorchMission
{
    [DisallowMultipleComponent]
    public sealed class TorchInteractable : MonoBehaviour
    {
        [SerializeField] private bool isLit;
        [SerializeField] private float interactionRadius = 2.25f;
        [SerializeField] private Color litColor = new Color(1f, 0.78f, 0.16f, 1f);
        [SerializeField] private Light flameLight;
        [SerializeField] private Renderer flameRenderer;
        [SerializeField] private Renderer[] torchRenderers = Array.Empty<Renderer>();

        public bool IsLit => isLit;
        public float InteractionRadius => interactionRadius;
        public Color LitColor => litColor;
        public Light FlameLight => flameLight;

        public void Configure(Light lightSource, Renderer flame, Renderer[] renderers, float radius)
        {
            flameLight = lightSource;
            flameRenderer = flame;
            torchRenderers = renderers ?? Array.Empty<Renderer>();
            interactionRadius = Mathf.Max(0.1f, radius);
            ApplyVisualState();
        }

        public bool CanLightFrom(Vector3 playerPosition)
        {
            return !isLit && Vector3.Distance(transform.position, playerPosition) <= interactionRadius;
        }

        public bool TryLight(Vector3 playerPosition)
        {
            if (!CanLightFrom(playerPosition))
            {
                return false;
            }

            LightTorch();
            return true;
        }

        public void LightTorch()
        {
            if (isLit)
            {
                return;
            }

            isLit = true;
            ApplyVisualState();
        }

        private void Awake()
        {
            ApplyVisualState();
        }

        private void OnValidate()
        {
            interactionRadius = Mathf.Max(0.1f, interactionRadius);
            ApplyVisualState();
        }

        private void ApplyVisualState()
        {
            if (flameLight != null)
            {
                flameLight.enabled = isLit;
                flameLight.color = litColor;
                flameLight.intensity = isLit ? 3.5f : 0f;
                flameLight.range = isLit ? 7f : 0f;
            }

            if (flameRenderer != null)
            {
                flameRenderer.enabled = isLit;
                SetRendererColor(flameRenderer, litColor, isLit ? 2f : 0f);
            }

            Color bodyColor = isLit ? litColor : new Color(0.23f, 0.14f, 0.08f, 1f);
            float emission = isLit ? 0.55f : 0f;
            for (int i = 0; i < torchRenderers.Length; i++)
            {
                SetRendererColor(torchRenderers[i], bodyColor, emission);
            }
        }

        private static void SetRendererColor(Renderer target, Color color, float emissionStrength)
        {
            if (target == null)
            {
                return;
            }

            Material material = target.material;
            material.color = color;
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", emissionStrength > 0f ? color * emissionStrength : Color.black);
                if (emissionStrength > 0f)
                {
                    material.EnableKeyword("_EMISSION");
                }
            }
        }
    }
}
