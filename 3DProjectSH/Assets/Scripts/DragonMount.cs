/*
 * used for mounting and dismounting the player
 * 
 * 
 */

using UnityEngine;
using UnityEngine.AI;

public class DragonMount : MonoBehaviour
{
    public Transform seatTransform;
    public DragonFlightController flightController;
    public NavMeshAgent agent;

    public int tamingCost = 10;
    [SerializeField] private bool hasBeenTamed = false;
    public bool HasBeenTamed => hasBeenTamed;

    public bool isMounted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flightController.enabled = false;

        if (agent != null) agent.enabled = true;
    }

    public void MarkTamed()
    {
        hasBeenTamed = true;
    }

    public bool MountPlayer(Transform playerTransform)
    {
        if (isMounted || !hasBeenTamed) return false;

        if (!HasBeenTamed)
        {
            if (GameManager.instance.mysticEnergy < tamingCost)
            {
                Debug.Log("Not enough energy");
                return false;
            }
            GameManager.instance.mysticEnergy -= tamingCost;
            hasBeenTamed = true;
        }

        isMounted = true;

        if (agent != null) agent.enabled = false;

        flightController.enabled = true;
        flightController.OnMount();

        playerTransform.SetParent(seatTransform);
        
        playerTransform.localRotation = Quaternion.identity;
        playerTransform.localPosition = Vector3.zero;
        
        playerTransform.localEulerAngles = new Vector3(-35f, 0f, 0f); // tilt down for dragon view

        return true;
    }

    public void DismountPlayer(Transform playerTransform, Transform originalParent)
    {
        if (!isMounted) return;
        isMounted = false;

        flightController.OnDismount();
        flightController.enabled = false;

        if (agent != null) agent.enabled = true;

        playerTransform.SetParent(originalParent);
        playerTransform.localPosition = Vector3.zero;
        playerTransform.localEulerAngles = Vector3.zero;

        //flightController.enabled = false;

        //isMounted = false;
    }
}
