using System;
using TorchMission;
using UnityEditor;
using UnityEngine;

namespace TorchMission.EditorTools
{
    public static class TorchMissionConsoleTestReport
    {
        [MenuItem("Tools/Torch Mission/Run Console Test Report")]
        public static void RunConsoleTestReport()
        {
            int passed = 0;
            int failed = 0;

            Debug.Log("========== TORCH SYSTEM TEST REPORT ==========");
            Debug.Log("Mission: Player walks near 3 torches and presses E to light them. Lit torches glow yellow and stay lit.");

            RunCheck("UNIT TEST - New torch starts unlit", () =>
            {
                TorchInteractable torch = TorchModelFactory.CreateTorch(Vector3.zero, Quaternion.identity, "Unit Torch");
                return !torch.IsLit && torch.FlameLight != null && !torch.FlameLight.enabled;
            }, ref passed, ref failed);

            RunCheck("UNIT TEST - Nearby player lights torch yellow", () =>
            {
                TorchInteractable torch = TorchModelFactory.CreateTorch(Vector3.zero, Quaternion.identity, "Unit Torch Near");
                bool result = torch.TryLight(new Vector3(0f, 0f, 1f));
                return result && torch.IsLit && torch.FlameLight.enabled && SameColor(torch.FlameLight.color, torch.LitColor);
            }, ref passed, ref failed);

            RunCheck("UNIT TEST - Far player cannot light torch", () =>
            {
                TorchInteractable torch = TorchModelFactory.CreateTorch(Vector3.zero, Quaternion.identity, "Unit Torch Far");
                bool result = torch.TryLight(new Vector3(0f, 0f, 10f));
                return !result && !torch.IsLit && !torch.FlameLight.enabled;
            }, ref passed, ref failed);

            RunCheck("INTEGRATION TEST - Player interactor lights nearest torch", () =>
            {
                TorchInteractable nearTorch = TorchModelFactory.CreateTorch(new Vector3(1f, 0f, 0f), Quaternion.identity, "Near Torch");
                TorchInteractable farTorch = TorchModelFactory.CreateTorch(new Vector3(8f, 0f, 0f), Quaternion.identity, "Far Torch");
                GameObject player = new GameObject("Integration Player");
                player.transform.position = Vector3.zero;
                PlayerTorchInteractor interactor = player.AddComponent<PlayerTorchInteractor>();
                bool result = interactor.TryInteractWithNearestTorch();
                return result && nearTorch.IsLit && !farTorch.IsLit;
            }, ref passed, ref failed);

            RunCheck("REGRESSION TEST - Lit torch stays lit after player walks away", () =>
            {
                TorchInteractable torch = TorchModelFactory.CreateTorch(Vector3.zero, Quaternion.identity, "Regression Torch");
                bool result = torch.TryLight(new Vector3(0f, 0f, 1f));
                Vector3 farAway = new Vector3(0f, 0f, 20f);
                return result && !torch.CanLightFrom(farAway) && torch.IsLit && torch.FlameLight.enabled;
            }, ref passed, ref failed);

            RunCheck("SMOKE TEST - Room builds with exactly 3 torches", () =>
            {
                TorchRoomBuilder.BuildRoom(includePlayer: false);
                TorchInteractable[] torches = UnityEngine.Object.FindObjectsByType<TorchInteractable>(FindObjectsSortMode.None);
                return torches.Length == TorchRoomBuilder.RequiredTorchCount;
            }, ref passed, ref failed);

            RunCheck("FUNCTIONAL TEST - Player can light all 3 torches", () =>
            {
                TorchRoomBuilder.BuildRoom(includePlayer: true);
                PlayerTorchInteractor interactor = UnityEngine.Object.FindFirstObjectByType<PlayerTorchInteractor>();
                TorchInteractable[] torches = UnityEngine.Object.FindObjectsByType<TorchInteractable>(FindObjectsSortMode.None);
                if (interactor == null || torches.Length != TorchRoomBuilder.RequiredTorchCount)
                {
                    return false;
                }

                for (int i = 0; i < torches.Length; i++)
                {
                    interactor.transform.position = torches[i].transform.position + new Vector3(0f, 0f, -1f);
                    if (!interactor.TryInteractWithNearestTorch())
                    {
                        return false;
                    }
                }

                for (int i = 0; i < torches.Length; i++)
                {
                    if (!torches[i].IsLit || !torches[i].FlameLight.enabled)
                    {
                        return false;
                    }
                }

                return true;
            }, ref passed, ref failed);

            CleanupTemporaryObjects();
            Debug.Log("========== TEST SUMMARY: " + passed + " PASSED, " + failed + " FAILED ==========");
            Debug.Log("Screenshot this Console output for Unit, Integration, Regression, Smoke, and Functional tests.");
        }

        [MenuItem("Tools/Torch Mission/Print TDD Fail Stage Screenshot Labels")]
        public static void PrintTddFailStageLabels()
        {
            Debug.LogError("TDD FAIL STAGE SCREENSHOT - UNIT TEST - New torch starts unlit - expected fail before implementation.");
            Debug.LogError("TDD FAIL STAGE SCREENSHOT - UNIT TEST - Nearby player lights torch yellow - expected fail before implementation.");
            Debug.LogError("TDD FAIL STAGE SCREENSHOT - UNIT TEST - Far player cannot light torch - expected fail before implementation.");
            Debug.Log("Screenshot these red Console entries for the requested TDD fail stage evidence.");
        }

        private static void RunCheck(string label, Func<bool> check, ref int passed, ref int failed)
        {
            CleanupTemporaryObjects();
            try
            {
                if (check())
                {
                    passed++;
                    Debug.Log("PASS - " + label);
                }
                else
                {
                    failed++;
                    Debug.LogError("FAIL - " + label);
                }
            }
            catch (Exception exception)
            {
                failed++;
                Debug.LogError("FAIL - " + label + " threw " + exception.GetType().Name + ": " + exception.Message);
            }
            finally
            {
                CleanupTemporaryObjects();
            }
        }

        private static void CleanupTemporaryObjects()
        {
            DestroyObjects(UnityEngine.Object.FindObjectsByType<TorchInteractable>(FindObjectsSortMode.None));
            DestroyObjects(UnityEngine.Object.FindObjectsByType<PlayerTorchInteractor>(FindObjectsSortMode.None));
            GameObject room = GameObject.Find("Torch Mission Room");
            if (room != null)
            {
                UnityEngine.Object.DestroyImmediate(room);
            }
        }

        private static void DestroyObjects<T>(T[] components) where T : Component
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(components[i].gameObject);
                }
            }
        }

        private static bool SameColor(Color a, Color b)
        {
            return Mathf.Approximately(a.r, b.r)
                && Mathf.Approximately(a.g, b.g)
                && Mathf.Approximately(a.b, b.b)
                && Mathf.Approximately(a.a, b.a);
        }
    }
}
