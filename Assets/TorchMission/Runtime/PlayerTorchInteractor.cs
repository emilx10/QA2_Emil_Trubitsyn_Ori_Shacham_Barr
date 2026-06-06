using UnityEngine;

namespace TorchMission
{
    [DisallowMultipleComponent]
    public sealed class PlayerTorchInteractor : MonoBehaviour
    {
        [SerializeField] private float searchRadius = 2.5f;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;
        [SerializeField] private Transform interactionOrigin;

        public TorchInteractable CurrentTorch { get; private set; }

        private void Awake()
        {
            if (interactionOrigin == null)
            {
                interactionOrigin = transform;
            }
        }

        private void Update()
        {
            CurrentTorch = FindNearestTorch();
            if (Input.GetKeyDown(interactionKey))
            {
                TryInteractWithNearestTorch();
            }
        }

        public bool TryInteractWithNearestTorch()
        {
            TorchInteractable torch = FindNearestTorch();
            CurrentTorch = torch;
            return torch != null && torch.TryLight(GetOriginPosition());
        }

        public TorchInteractable FindNearestTorch()
        {
            TorchInteractable[] torches = FindObjectsByType<TorchInteractable>(FindObjectsSortMode.None);
            TorchInteractable nearest = null;
            float nearestDistance = float.PositiveInfinity;
            Vector3 origin = GetOriginPosition();

            for (int i = 0; i < torches.Length; i++)
            {
                TorchInteractable torch = torches[i];
                if (torch == null || torch.IsLit)
                {
                    continue;
                }

                float distance = Vector3.Distance(origin, torch.transform.position);
                if (distance <= Mathf.Min(searchRadius, torch.InteractionRadius) && distance < nearestDistance)
                {
                    nearest = torch;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        private Vector3 GetOriginPosition()
        {
            return interactionOrigin != null ? interactionOrigin.position : transform.position;
        }
    }
}
