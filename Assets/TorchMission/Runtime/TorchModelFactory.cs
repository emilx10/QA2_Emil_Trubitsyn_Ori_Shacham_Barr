using UnityEngine;

namespace TorchMission
{
    public static class TorchModelFactory
    {
        public static TorchInteractable CreateTorch(Vector3 position, Quaternion rotation, string name = "Torch")
        {
            GameObject root = new GameObject(name);
            root.transform.SetPositionAndRotation(position, rotation);

            GameObject backPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backPlate.name = "Wall Plate";
            backPlate.transform.SetParent(root.transform, false);
            backPlate.transform.localPosition = new Vector3(0f, 0.7f, 0f);
            backPlate.transform.localScale = new Vector3(0.32f, 0.48f, 0.08f);
            Renderer plateRenderer = backPlate.GetComponent<Renderer>();
            plateRenderer.material = CreateMaterial(new Color(0.15f, 0.11f, 0.08f, 1f));

            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Wood Handle";
            handle.transform.SetParent(root.transform, false);
            handle.transform.localPosition = new Vector3(0f, 0.25f, -0.32f);
            handle.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            handle.transform.localScale = new Vector3(0.08f, 0.65f, 0.08f);
            Renderer handleRenderer = handle.GetComponent<Renderer>();
            handleRenderer.material = CreateMaterial(new Color(0.24f, 0.14f, 0.07f, 1f));

            GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.name = "Yellow Flame";
            flame.transform.SetParent(root.transform, false);
            flame.transform.localPosition = new Vector3(0f, 0.95f, -0.53f);
            flame.transform.localScale = new Vector3(0.28f, 0.42f, 0.28f);
            Renderer flameRenderer = flame.GetComponent<Renderer>();
            flameRenderer.material = CreateMaterial(new Color(1f, 0.72f, 0.08f, 1f));

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.shadows = LightShadows.Soft;

            TorchInteractable torch = root.AddComponent<TorchInteractable>();
            torch.Configure(light, flameRenderer, new[] { plateRenderer, handleRenderer }, 2.25f);
            return torch;
        }

        public static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
