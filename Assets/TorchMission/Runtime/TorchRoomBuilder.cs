using UnityEngine;
using UnityEngine.SceneManagement;

namespace TorchMission
{
    public static class TorchRoomBuilder
    {
        public const int RequiredTorchCount = 3;

        public static GameObject BuildRoom(bool includePlayer = true)
        {
            GameObject root = new GameObject("Torch Mission Room");
            CreateFloor(root.transform);
            CreateWall(root.transform, "Back Wall", new Vector3(0f, 2f, 5f), new Vector3(12f, 4f, 0.25f));
            CreateWall(root.transform, "Left Wall", new Vector3(-6f, 2f, 0f), new Vector3(0.25f, 4f, 10f));
            CreateWall(root.transform, "Right Wall", new Vector3(6f, 2f, 0f), new Vector3(0.25f, 4f, 10f));
            CreateWall(root.transform, "Ceiling", new Vector3(0f, 4f, 0f), new Vector3(12f, 0.18f, 10f));

            TorchModelFactory.CreateTorch(new Vector3(-4f, 1.25f, 4.82f), Quaternion.identity, "Torch 1").transform.SetParent(root.transform);
            TorchModelFactory.CreateTorch(new Vector3(0f, 1.25f, 4.82f), Quaternion.identity, "Torch 2").transform.SetParent(root.transform);
            TorchModelFactory.CreateTorch(new Vector3(4f, 1.25f, 4.82f), Quaternion.identity, "Torch 3").transform.SetParent(root.transform);

            CreateAmbientLight(root.transform);
            if (includePlayer)
            {
                CreatePlayer(root.transform);
            }

            return root;
        }

        private static void CreateFloor(Transform parent)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Stone Floor";
            floor.transform.SetParent(parent, false);
            floor.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(12f, 0.1f, 10f);
            floor.GetComponent<Renderer>().material = TorchModelFactory.CreateMaterial(new Color(0.28f, 0.29f, 0.27f, 1f));
        }

        private static void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent, false);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = TorchModelFactory.CreateMaterial(new Color(0.34f, 0.32f, 0.29f, 1f));
        }

        private static void CreateAmbientLight(Transform parent)
        {
            GameObject lightObject = new GameObject("Dim Room Fill Light");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.localPosition = new Vector3(0f, 3.2f, -2f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.45f, 0.52f, 0.62f, 1f);
            light.intensity = 0.35f;
            light.range = 9f;
        }

        private static void CreatePlayer(Transform parent)
        {
            GameObject player = new GameObject("Player");
            player.transform.SetParent(parent, false);
            player.transform.localPosition = new Vector3(0f, 1f, -2.8f);
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            player.AddComponent<PlayerTorchInteractor>();

            GameObject cameraObject = new GameObject("Player Camera");
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.nearClipPlane = 0.05f;
            cameraObject.AddComponent<AudioListener>();

            SimpleFirstPersonController firstPersonController = player.AddComponent<SimpleFirstPersonController>();
            firstPersonController.SetCamera(camera);
        }
    }

    public sealed class TorchMissionRuntimeBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildMissionOnPlay()
        {
            if (Object.FindFirstObjectByType<TorchInteractable>() != null)
            {
                return;
            }

            TorchRoomBuilder.BuildRoom();
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.09f, 1f);
        }
    }
}
