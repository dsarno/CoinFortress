using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("CollisionTest started");
        
        // Check if we have required components
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("No Collider2D found on " + gameObject.name);
        }
        else
        {
            Debug.Log($"Found {collider.GetType().Name} on {gameObject.name}, isTrigger: {collider.isTrigger}");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"COLLISION TEST: {gameObject.name} hit by {other.gameObject.name}");
    }
}