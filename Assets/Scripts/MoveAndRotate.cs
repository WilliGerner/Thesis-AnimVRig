using UnityEngine;

public class MoveAndRotate : MonoBehaviour
{
    public float speed = 1.0f;          // Geschwindigkeit der Bewegung
    public float rotationSpeed = 50.0f; // Geschwindigkeit der Rotation

    void Update()
    {
        // Bewegt das GameObject nach vorne (relativ zu seiner aktuellen Ausrichtung)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Rotiert das GameObject um seine Y-Achse
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
