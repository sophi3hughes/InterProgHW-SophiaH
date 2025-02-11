/*Objectives Met in this Script:
 * Enum to manage popcorn states
 * Switch statements x3
 * array for child components of popcorn prefab
 * prefabs made for popcorn and feedback text
 * NEW: enum for normal or rainbow popcorn type
*/

using Unity.VisualScripting;
using UnityEngine;

public class Popcorn : MonoBehaviour
{
    // ENUM
    public enum PopcornStates
    {
        Kernel,
        WellPopped,
        PerfectlyPopped,
        Burnt,
        Rainbow
    }

    public enum PopcornVariant
    {
        Normal,
        Rainbow
    }
    
    public PopcornStates currentState = PopcornStates.Kernel;
    public PopcornVariant variant = PopcornVariant.Normal;

    // settings
    public float stateDuration = 0.5f;
    public float stateTimer = 0f;
    private bool hasBeenClicked = false;

    // materials
    public Material kernel;
    public Material wellPopped;
    public Material perfectlyPopped;
    public Material burnt;
    public Material rainbow;

    // child meshes
    public GameObject kernelModel;
    public GameObject poppedModel;

    [SerializeField]
    private PopcornManager manager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = Object.FindFirstObjectByType<PopcornManager>();

        Rigidbody rb = GetComponent<Rigidbody>();
        
        // randomize pop direction
        Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
        float forceAmount = Random.Range(200f, 600f);
        rb.AddForce(randomDirection * forceAmount);
        // random spinning
        rb.AddTorque(Random.insideUnitSphere * 50f);

        if (variant == PopcornVariant.Rainbow)
        {
            currentState = PopcornStates.Rainbow;
            stateDuration = 1.5f;
        }
        else
        {
            int randomIndex = Random.Range(0, 4);
            currentState = (PopcornStates)randomIndex;
        }

        UpdatePopcorn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasBeenClicked && currentState != PopcornStates.Burnt)
        {
            float speedMult = (variant == PopcornVariant.Rainbow) ? 1f : manager.popcornSpeedMultiplier; // TERNARY OP

            stateTimer += Time.deltaTime * speedMult;
            if (stateTimer >= stateDuration)
            {
                MoveToNextState();
                stateTimer = 0f;
            }
        }
    }

    // SWITCH statement
    private void MoveToNextState()
    {
        if(variant == PopcornVariant.Rainbow)
        {
            currentState = PopcornStates.Burnt;
        }
        else
        {
            switch (currentState)
            {
                case PopcornStates.Kernel:
                    currentState = PopcornStates.WellPopped;
                    break;
                case PopcornStates.WellPopped:
                    currentState = PopcornStates.PerfectlyPopped;
                    break;
                case PopcornStates.PerfectlyPopped:
                    currentState = PopcornStates.Burnt;
                    break;
                default:
                    break;
            }
        }
        UpdatePopcorn();
    }

    private void OnMouseDown()
    {
        if (hasBeenClicked) return;

        hasBeenClicked = true;

        int scoreChange = 0;
        string feedback = "";
        if (variant  == PopcornVariant.Rainbow && currentState != PopcornStates.Burnt)
        {
            scoreChange = 100;
            feedback = "Rainbow! +100";
            manager.SlowPopcornStates(7f);
        }
        else
        {
            switch (currentState)
            {
                case PopcornStates.Kernel:
                    scoreChange = -5;
                    feedback = "Kernel! -5";
                    break;
                case PopcornStates.WellPopped:
                    scoreChange = 5;
                    feedback = "Good! +5";
                    break;
                case PopcornStates.PerfectlyPopped:
                    scoreChange = 15;
                    feedback = "Perfectly Popped! +15";
                    break;
                case PopcornStates.Burnt:
                    scoreChange = -10;
                    feedback = "Burnt! -10";
                    break;
                default:
                    break;
            }
        }

        manager.CheckCombo(currentState, variant, ref scoreChange);
        manager.AddPopcornCount(currentState, variant);

        manager.UpdateScore(scoreChange);
        manager.SpawnText(feedback, transform.position);

        // destroy after delay
        Destroy(gameObject, 0.1f);
    }

    private void UpdatePopcorn()
    {
        bool isKernel = (currentState == PopcornStates.Kernel);
        kernelModel.SetActive(isKernel);
        poppedModel.SetActive(!isKernel);

        Material mat = null;

        if (variant == PopcornVariant.Rainbow && currentState != PopcornStates.Burnt)
        {
            mat = rainbow;
        }
        else
        {
            switch (currentState)
            {
                case PopcornStates.Kernel:
                    mat = kernel;
                    break;
                case PopcornStates.WellPopped:
                    mat = wellPopped;
                    break;
                case PopcornStates.PerfectlyPopped:
                    mat = perfectlyPopped;
                    break;
                case PopcornStates.Burnt:
                    mat = burnt;
                    break;
                default:
                    break;
            }
        }

        if (isKernel)
        {
            Renderer r = kernelModel.GetComponent<Renderer>();
            r.material = mat;
        }
        else
        {
            // ARRAY
            Renderer[] renderers = poppedModel.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in renderers)
            {
                r.material = mat;
            }
        }
    }
}
