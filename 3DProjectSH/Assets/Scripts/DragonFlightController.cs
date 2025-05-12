/*
 * Dragon flight script
 * - Controls: mouse direction to go specific direction, w to go forwad, space to ascend, right click to speed boost, left shift dismount
 * - added CAMERA effects while flying: fov zoom out when speed boosting(right click), slight camera bob while flying, camera tilt
 * - animations: used different animations from the 3d model for landing, take off, flying, gliding, etc
 */

using UnityEngine;

public class DragonFlightController : MonoBehaviour
{
    [Header("Speeds")]
    public float maxForwardSpeed = 20f;
    public float groundForwardSpeed = 8f;
    public float verticalSpeed = 5f;
    public float acceleration = 10f;

    [Header("Turning & Pitch")]
    public float turnSpeed = 90f;
    public float maxPitch = 80f;
    public float maxRoll = 30f;
    public float rollSpeed = 2f;

    [Header("Animator / Layers")]
    public Animator dragonAnimator;
    public LayerMask groundLayer;

    [Header("Camera Bobbing")]
    public Transform cameraTransform;
    public float bobFrequency = 1f;
    public float bobAmplitude = 0.05f;

    [Header("Speed Boost")]
    public float boostMultiplier = 1.5f;
    public float boostDuration = 2f;
    public float boostCooldown = 3f;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float nextBoostTime = 0f;

    [Header("FOV Boost")]
    public Camera mainCam;
    public float normalFov = 60f;
    public float boostedFov = 75f;
    public float fovLerpSpeed = 2f;

    // flight states
    private float currentPitch = 0f;
    private float currentYaw = 0f;
    private float currentRoll = 0f;
    private Vector3 velocity;
    private float distanceToGround = 999f;

    private bool isGrounded = true;
    private bool wasGroundedLastFrame = true;
    public bool hasRider = false;
    public float takeOffBoost = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource wingAudio;

    void Start()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (mainCam != null)
        {
            mainCam.fieldOfView = normalFov;
        }
    }

    void Update()
    {
        // default to idle
        if (!hasRider)
        {
            isGrounded = true;
            velocity = Vector3.zero;
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
            //ForceIdleAnimator();
            return;
        }

        // right click for speed boost
        if (!isGrounded && Input.GetMouseButtonDown(1) && Time.time >= nextBoostTime)
        {
            isBoosting = true;
            boostTimer = boostDuration;
            nextBoostTime = Time.time + boostCooldown;
        }

        // timer for speed boost
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }

        // flying
        isGrounded = CheckGrounded();
        HandleRotation();
        HandleMovement();
        transform.Translate(velocity * Time.deltaTime, Space.Self);

        // animator
        bool isFlying = !isGrounded;
        float speedInXZ = new Vector2(velocity.x, velocity.z).magnitude;
        bool isMoving = (speedInXZ > 0.1f);
        bool isTurning = Mathf.Abs(Input.GetAxis("Mouse X")) > 0.5f;

        // landing and take off
        if (!wasGroundedLastFrame && isGrounded)
        {
            dragonAnimator.SetTrigger("LandTrigger");
        }
        else if (wasGroundedLastFrame && !isGrounded)
        {
            if (velocity.y > 0.01f)
            {
                dragonAnimator.SetTrigger("TakeOffTrigger");
            }
            else
            {
                isGrounded = true;
            }
        }
        wasGroundedLastFrame = isGrounded;

        // animator
        dragonAnimator.SetBool("IsGrounded", isGrounded);
        dragonAnimator.SetBool("IsMoving", isMoving);
        dragonAnimator.SetBool("IsFlying", isFlying);
        dragonAnimator.SetBool("IsTurning", isTurning);

        // camera bob
        if (cameraTransform != null && isFlying && isMoving)
        {
            float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            Vector3 localPos = cameraTransform.localPosition;
            localPos.y = bobOffset;
            cameraTransform.localPosition = localPos;
        }

        HandleCameraFOV();

        if (wingAudio != null)
        {
            bool shouldPlay = isFlying && isMoving;

            if (shouldPlay && !wingAudio.isPlaying)
                wingAudio.Play();
            else if (!shouldPlay && wingAudio.isPlaying)
                wingAudio.Stop();

            wingAudio.pitch = isBoosting ? 1.25f : 1f;
            wingAudio.volume = isBoosting ? 1.0f : 0.8f;
        }
    }

    private void HandleCameraFOV()
    {
        if (mainCam == null) return;

        float targetFov = isBoosting ? boostedFov : normalFov;
        float currentFov = mainCam.fieldOfView;
        // lerp fov
        float newFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovLerpSpeed);
        mainCam.fieldOfView = newFov;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float yawChange = mouseX * turnSpeed * Time.deltaTime;
        currentYaw += yawChange;

        if (isGrounded)
        {
            currentPitch = 0f;
            currentRoll = 0f;
        }
        else
        {
            // turning in air
            float pitchChange = -mouseY * turnSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch + pitchChange, -maxPitch, maxPitch);

            float targetRoll = -mouseX * maxRoll;
            currentRoll = Mathf.MoveTowards(currentRoll, targetRoll, rollSpeed * Time.deltaTime);
        }

        transform.localEulerAngles = new Vector3(currentPitch, currentYaw, currentRoll);
    }

    private void HandleMovement()
    {
        // take off from ground when space
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            velocity.y = takeOffBoost;
        }

        float forwardInput = Input.GetKey(KeyCode.W) ? 1f : 0f;
        float desiredForwardSpeed = isGrounded ? groundForwardSpeed : maxForwardSpeed;

        // speed multiplier
        if (!isGrounded && isBoosting)
        {
            desiredForwardSpeed *= boostMultiplier;
        }

        float targetSpeedZ = desiredForwardSpeed * forwardInput;
        float speedZ = velocity.z;
        speedZ = Mathf.MoveTowards(speedZ, targetSpeedZ, acceleration * Time.deltaTime);

        if (isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            // ascend/descend
            float verticalInput = 0f;
            if (Input.GetKey(KeyCode.Space)) verticalInput = 1f;
            else if (Input.GetKey(KeyCode.LeftControl)) verticalInput = -1f;

            float targetVerticalSpeed = verticalSpeed * verticalInput;
            velocity.y = Mathf.MoveTowards(velocity.y, targetVerticalSpeed, acceleration * Time.deltaTime);
        }

        velocity.x = 0f;
        velocity.z = speedZ;
    }

    private bool CheckGrounded()
    {
        float maxDistance = 50f;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            distanceToGround = hit.distance;
            if (distanceToGround < 2f && velocity.y <= 0f)
            {
                return true;
            }
        }
        else
        {
            distanceToGround = 999f;
        }
        return false;
    }

    public void OnMount()
    {
        hasRider = true;
    }

    public void OnDismount()
    {
        hasRider = false;
        velocity = Vector3.zero;
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        //ForceIdleAnimator();

        if (mainCam != null)
        {
            mainCam.fieldOfView = normalFov;
        }
    }

    private void ForceIdleAnimator()
    {
        dragonAnimator.SetBool("IsGrounded", true);
        dragonAnimator.SetBool("IsMoving", false);
        dragonAnimator.SetBool("IsFlying", false);
        dragonAnimator.SetBool("IsTurning", false);
    }
}

