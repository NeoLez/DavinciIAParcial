using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float speed;

    void Update() {
        var movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate(movementVector  * (speed * Time.deltaTime));
    }
}
