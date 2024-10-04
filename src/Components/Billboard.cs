using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    bool flip = false;

    [SerializeField]
    float moveSpeed = .2f;

    Camera cam;

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(
                    flip ? -cam.transform.forward : cam.transform.forward,
                    cam.transform.up
                ),
                moveSpeed
            );
        }
    }
}
