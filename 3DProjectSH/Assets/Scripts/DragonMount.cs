using UnityEngine;

public class DragonMount : MonoBehaviour
{
    public Transform seatTransform;
    public DragonFlightController flightController;

    private bool isMounted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flightController.enabled = false;
    }
    public void MountPlayer(Transform playerTransform)
    {
        if (isMounted) return;

        flightController.enabled = true;
        flightController.OnMount();

        playerTransform.SetParent(seatTransform);
        
        playerTransform.localRotation = Quaternion.identity;
        playerTransform.localPosition = Vector3.zero;
        
        playerTransform.localEulerAngles = new Vector3(-35f, 0f, 0f); // tilt down for dragon view

        isMounted = true;
    }

    public void DismountPlayer(Transform playerTransform, Transform originalParent)
    {
        if (!isMounted) return;

        flightController.OnDismount();

        playerTransform.SetParent(originalParent);
        playerTransform.localPosition = Vector3.zero;
        playerTransform.localEulerAngles = Vector3.zero;

        flightController.enabled = false;

        isMounted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
