using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GroundScript : MonoBehaviour
{
    public float moveSpeed; 
    public float groundWidth;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Speichere die Startposition
    }

    void Update()
    {
        // Bewege den Boden nach links
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Wenn der Boden den linken Rand verlässt, setze ihn zurück nach rechts
        if (transform.position.x < startPos.x - groundWidth)
        {
            transform.position = startPos;
        }
    }
}
