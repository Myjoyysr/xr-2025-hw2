using UnityEngine;

public class LensCameraMovement : MonoBehaviour
{
    public Transform mainCamera;
    public Transform lens;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 mainCameraPosition = (lens.InverseTransformPoint(mainCamera.position));

        Vector3 lookDir = lens.TransformPoint(new Vector3(-mainCameraPosition.x,-mainCameraPosition.y,-mainCameraPosition.z));

        transform.position = lens.position;
        transform.LookAt(lookDir, lens.up);

        
    }
}
