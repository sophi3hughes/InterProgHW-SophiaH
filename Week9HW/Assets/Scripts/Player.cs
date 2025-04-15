using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform; // Assign your camera here in the inspector

    [Header("Shooting")]
    public GameObject bulletPrefab;   // Assign a bullet prefab here
    public Transform bulletSpawnPoint; // A child Transform in front of the camera

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f;

    void Start()
    {
        // Lock and hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        LookAround();
        MovePlayer();
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up/down (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player body left/right (yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        // Jump (simple version)
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // small downward force to keep grounded
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // Apply gravity
            verticalVelocity -= gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    void Shoot()
    {
        if (bulletPrefab && bulletSpawnPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            // You can add velocity or a bullet script to move it
        }
    }
}
