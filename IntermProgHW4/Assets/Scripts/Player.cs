using UnityEngine;

namespace StealthGame
{
    public class Player : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float gravity = -9.81f;

        [Header("Camera Settings")]
        public float mouseSensitivity = 2f;
        public Transform cameraTransform;

        private CharacterController controller;
        private float verticalVelocity;
        private float cameraPitch = 0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);

            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            verticalVelocity += gravity * Time.deltaTime;
            moveDirection.y = verticalVelocity;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}