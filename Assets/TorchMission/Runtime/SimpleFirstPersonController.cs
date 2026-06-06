using UnityEngine;

namespace TorchMission
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class SimpleFirstPersonController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float mouseSensitivity = 2.8f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private bool lockCursorOnPlay = true;

        private CharacterController controller;
        private float cameraPitch;
        private float verticalVelocity;

        public void SetCamera(Camera camera)
        {
            playerCamera = camera;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }
        }

        private void Start()
        {
            if (lockCursorOnPlay)
            {
                LockCursor();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && lockCursorOnPlay)
            {
                LockCursor();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnlockCursor();
            }
            else if (Input.GetMouseButtonDown(0) && lockCursorOnPlay)
            {
                LockCursor();
            }

            MovePlayer();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                LookAround();
            }
        }

        private void MovePlayer()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 planarVelocity = (transform.right * horizontal + transform.forward * vertical).normalized * moveSpeed;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime;
            controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        private void LookAround()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            transform.Rotate(0f, mouseX, 0f, Space.Self);

            if (playerCamera != null)
            {
                cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -80f, 80f);
                playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }

        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
