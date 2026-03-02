using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 upperBounds;
    [SerializeField] private Vector2 lowerBounds;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float smoothing;
    [SerializeField] private Camera cam;
    [SerializeField] private float zOffset;
    
    private Vector2 targetPos;
    private float targetSize;

    private void Start()
    {
        targetPos = cam.transform.position;
        targetSize = cam.orthographicSize;
        prevMousePos = Input.mousePosition;
    }

    private Vector2 prevMousePos;
    private void Update()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            
            float newSize = targetSize - Input.mouseScrollDelta.y * zoomSpeed;
            newSize = Mathf.Max(newSize, minSize);
            newSize = Mathf.Min(newSize, maxSize);
            
            Vector2 camMouseVector = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - targetPos;
            Vector2 percentage = camMouseVector / new Vector2(targetSize * cam.aspect, targetSize);
            Vector2 newCamMouseVector = percentage * new Vector2(newSize * cam.aspect, newSize);
            targetPos += camMouseVector - newCamMouseVector;
            
            targetSize = newSize;
        }

        if (Input.GetMouseButton(2))
        {
            targetPos -= ((Vector2)Input.mousePosition - prevMousePos) * (movementSpeed * cam.orthographicSize);
        }

        prevMousePos = Input.mousePosition;

        cam.transform.position = Vector2.Lerp(cam.transform.position, targetPos, smoothing);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothing);

        Vector2 camUpperBounds = new Vector2(cam.transform.position.x + cam.orthographicSize * cam.aspect, cam.transform.position.y + cam.orthographicSize);
        Vector2 camLowerBounds = new Vector2(cam.transform.position.x - cam.orthographicSize * cam.aspect, cam.transform.position.y - cam.orthographicSize);
        Vector2 upperBoundDiff = upperBounds - camUpperBounds;
        Vector2 lowerBoundDiff = camLowerBounds - lowerBounds;
        Vector2 upperDiff = Vector2.Min(Vector2.zero, upperBoundDiff);
        Vector2 lowerDiff = Vector2.Min(Vector2.zero, lowerBoundDiff);
        cam.transform.position += (Vector3)upperDiff;
        cam.transform.position -= (Vector3)lowerDiff;
        cam.transform.position += Vector3.forward * zOffset;
        
        targetPos += upperDiff;
        targetPos -= lowerDiff;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(upperBounds.x, upperBounds.y), new Vector3(upperBounds.x, lowerBounds.y));
        Gizmos.DrawLine(new Vector3(upperBounds.x, lowerBounds.y), new Vector3(lowerBounds.x, lowerBounds.y));
        Gizmos.DrawLine(new Vector3(lowerBounds.x, lowerBounds.y), new Vector3(lowerBounds.x, upperBounds.y));
        Gizmos.DrawLine(new Vector3(lowerBounds.x, upperBounds.y), new Vector3(upperBounds.x, upperBounds.y));
    }
}
