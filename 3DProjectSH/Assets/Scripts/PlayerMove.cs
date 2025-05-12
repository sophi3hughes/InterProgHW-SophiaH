/*
 * For player movement. WASD, space for jump. F to interact. left shift to dismount dragon
 * - using CharacterController
 * - using raycasts to check distances
 * - 
 */
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private CharacterController characterController;
    private float walkSpeed = 15f;
    private float sprintSpeed = 30f;
    private float mouseSensitivity = 3.5f;

    Transform cameraTrans;
    float cameraPitch = 0;

    private Vector3 defaultCameraLocalPos;
    private Quaternion defaultCameraLocalRot;

    float gravityValue = -35;
    float jumpHeight = 4f;
    float currentYVelocity;

    public float raycastRange = 5f;
    public KeyCode interactKey = KeyCode.F;

    public bool isRidingDragon = false;
    private DragonMount currentDragonMount;
    private Transform originalCameraParent;

    int groundLayer = 1 << 6;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTrans = Camera.main.transform;

        defaultCameraLocalPos = cameraTrans.localPosition;
        defaultCameraLocalRot = cameraTrans.localRotation;

        originalCameraParent = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isRidingDragon)
        {
            HandleCamera();
            HandleMovement();
            HandleInteraction();
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) && IsGroundNearby())
            {
                DismountFromDragon();
            }
        }
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // rotate the player left/right
        transform.Rotate(Vector3.up * mouseX);

        // rotate the camera up/down
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTrans.localEulerAngles = Vector3.right * cameraPitch;
    }

    private void HandleMovement()
    {
        /*float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        
        Vector3 move = transform.rotation *
            new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // jump/gravity logic
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentYVelocity += Mathf.Sqrt(2f * jumpHeight * -gravityValue);
            }
            else
            {
                currentYVelocity = -0.5f;
            }
        }
        else
        {
            currentYVelocity += gravityValue * Time.deltaTime;
        }

        move.y = currentYVelocity;

        characterController.Move(move * Time.deltaTime * currentSpeed);*/

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 horizontal = transform.rotation * input;
        if (horizontal.magnitude > 1f)
            horizontal = horizontal.normalized;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        horizontal *= currentSpeed;

        // jump
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentYVelocity = Mathf.Sqrt(5f * jumpHeight * -gravityValue);
            }
            else
            {
                currentYVelocity = -0.5f;
            }
        }
        else
        {
            currentYVelocity += gravityValue * Time.deltaTime;
        }

        Vector3 finalMove = horizontal;
        finalMove.y = currentYVelocity;
        characterController.Move(finalMove * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        Ray ray = new Ray(cameraTrans.position, cameraTrans.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            if (Input.GetKeyDown(interactKey))
            {
                /*if (hit.transform.CompareTag("Collectible"))
                {
                    Debug.Log("You picked up: " + hit.transform.name);
                    GameManager.instance.collectibleCount++;
                    GameManager.instance.mysticEnergy += 5;

                    Destroy(hit.transform.gameObject);
                }*/
                if (hit.transform.CompareTag("Teleporter"))
                {
                    Debug.Log("Teleporting to next scene...");
                    //SceneManager.LoadScene("MyOtherScene");
                }
                else if (hit.transform.CompareTag("Dragon"))
                {
                    DragonMount dragonMount = hit.transform.GetComponent<DragonMount>();
                    MountDragon(dragonMount);
                }
            }
        }
    }

    private bool IsGroundNearby(float maxDistance = 30f)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundLayer))
        {
            Debug.Log("Hit object: " + hit.collider.name + " on layer " + hit.collider.gameObject.layer);
            return true;
        }
        return false;
    }

    private void MountDragon(DragonMount dragonMount)
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        cameraPitch = 0f;
        cameraTrans.localEulerAngles = Vector3.zero;

        if (dragonMount.MountPlayer(transform))
        {
            characterController.enabled = false;
            currentDragonMount = dragonMount;
            isRidingDragon = true;
        }
        /*characterController.enabled = false;
        //this.enabled = false;
        dragonMount.MountPlayer(transform);

        currentDragonMount = dragonMount;
        isRidingDragon = true;*/
        //dragonMount.flightController.OnMount();
    }

    private void DismountFromDragon()
    {
        characterController.enabled = true;

        //this.enabled = true;

        if (currentDragonMount != null)
        {
            Transform dragonSeat = currentDragonMount.seatTransform;

            currentDragonMount.DismountPlayer(transform, originalCameraParent);

            Vector3 dismountOffset = dragonSeat.right * 2f + Vector3.up * 0.5f;
            transform.position = dragonSeat.position + dismountOffset;

            //transform.rotation = Quaternion.LookRotation(dragonSeat.forward, Vector3.up);
            cameraTrans.localPosition = defaultCameraLocalPos;
            cameraTrans.localRotation = defaultCameraLocalRot;

            currentDragonMount = null;
        }

        isRidingDragon = false;
        //currentDragonMount.flightController.OnDismount();
    }

}
