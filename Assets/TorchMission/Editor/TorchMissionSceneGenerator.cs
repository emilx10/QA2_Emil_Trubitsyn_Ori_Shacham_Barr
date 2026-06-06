using TorchMission;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TorchMission.EditorTools
{
    public static class TorchMissionSceneGenerator
    {
        private const string ScenePath = "Assets/TorchMission/TorchMissionScene.unity";

        [MenuItem("Tools/Torch Mission/Generate Scene")]
        public static void GenerateScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TorchMissionScene";
            TorchRoomBuilder.BuildRoom(includePlayer: true);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.09f, 1f);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("Generated Torch Mission scene at " + ScenePath);
        }
    }
}
