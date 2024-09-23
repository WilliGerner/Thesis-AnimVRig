using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 1.0f;  // Geschwindigkeit des Modells beim Gehen
    public Transform checkpoint;  // Transform f�r den ersten Checkpoint (nur Z-Achse wird verwendet)
    public Transform checkpointTwo;  // Transform f�r den zweiten Checkpoint (nur Z-Achse wird verwendet)
    public string nextAnimationName = "Turn";  // Name der Animation, die nach dem Checkpoint abgespielt wird

    public Animator animator;
    public float animationSpeed = 1.0f;  // Steuerung der Abspielgeschwindigkeit der Animation

    private bool isWalking = true;
    private bool reachedCheckpointOne = false;

    void Start()
    {
        // Setzt die Anfangsgeschwindigkeit des Animators
        animator.speed = animationSpeed;
    }

    void Update()
    {
        if (isWalking)
        {
            CorrectAnimatorRotation();  // Ausrichtung des Animator-Objekts korrigieren

            if (!reachedCheckpointOne)
            {
                // Bewegung des GameObjects entlang der Z-Achse nach vorne
                transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            }
            else
            {
                // Bewegung des GameObjects entlang der Z-Achse zur�ck
                transform.Translate(Vector3.back * walkSpeed * Time.deltaTime);
            }

            // �berpr�fen, ob das GameObject den ersten Checkpoint erreicht hat (nur Z-Achse)
            if (!reachedCheckpointOne && transform.position.z >= checkpoint.position.z)
            {
                Debug.Log("ReachedCheckpoint");
                StopWalkingAndStartAnimation(true);
            }
            // �berpr�fen, ob das GameObject den zweiten Checkpoint erreicht hat (nur Z-Achse)
            else if (reachedCheckpointOne && transform.position.z <= checkpointTwo.position.z)
            {
                Debug.Log("ReachedCheckpointTwo");
                StopWalkingAndStartAnimation(false);
            }
            else
            {
                animator.SetBool("Walk", true);  // Setzt eine Bedingung f�r den Animator
            }
        }

        // Aktualisiert die Geschwindigkeit des Animators basierend auf der public Variable
        animator.speed = animationSpeed;
    }

    void CorrectAnimatorRotation()
    {
        // Holt die aktuelle Rotation des Animator-Objekts und rundet sie auf die n�chste "gerade" Richtung (0�, 90�, 180�, 270�)
        Vector3 currentRotation = animator.transform.eulerAngles;
        currentRotation.y = Mathf.Round(currentRotation.y / 90) * 90;
        animator.transform.eulerAngles = currentRotation;
    }

    void StopWalkingAndStartAnimation(bool firstCheckpoint)
    {
        isWalking = false;
        animator.SetBool("Walk", false);  // Stoppt die Geh-Animation
        animator.applyRootMotion = true;  // Aktiviert Root Motion f�r die �bergangsanimation
        Debug.Log("Set RootMotion True");
        animator.Play(nextAnimationName);  // Wechselt zur n�chsten Animation (z.B. Drehung)

        if (firstCheckpoint)
        {
            StartCoroutine(WaitForAnimationEndAndResumeWalking(true));
        }
        else
        {
            StartCoroutine(WaitForAnimationEndAndResumeWalking(false));
        }
    }

    IEnumerator WaitForAnimationEndAndResumeWalking(bool firstCheckpoint)
    {
        // Warte, bis die Animation abgeschlossen ist, basierend auf der L�nge der Animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / animator.speed);

        Debug.Log("Set RootMotion FALSE");
        animator.applyRootMotion = false;  // Deaktiviert Root Motion f�r die Geh-Animation
        isWalking = true;
        animator.SetBool("Walk", true);  // Setzt die Geh-Animation wieder auf

        if (firstCheckpoint)
        {
            reachedCheckpointOne = true;  // Nach dem ersten Checkpoint, zur�cklaufen
        }
        else
        {
            reachedCheckpointOne = false;  // Nach dem zweiten Checkpoint, wieder zum ersten Checkpoint laufen
        }
    }
}
