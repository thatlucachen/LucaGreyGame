using UnityEngine;

public class BaseballMovementLeft : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] float movementAmount = 0;
    [SerializeField] float movementSpeed = 3;
    [SerializeField] float movementTime = 3;
    Vector2 spawnPosition;
    [SerializeField] float currentMovementTime = 0;
    [SerializeField] bool shouldMoveRight = false;
    [SerializeField] bool hasHitPlayer = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spawnPosition = new Vector2(transform.position.x, transform.position.y);
    }
    
    private void Update()
    {
        currentMovementTime += Time.deltaTime;

        if (currentMovementTime < movementTime && !hasHitPlayer)
        {
            if (shouldMoveRight)
            {
                movementAmount = movementSpeed * Time.deltaTime;
                transform.position = new Vector2(transform.position.x + movementAmount, transform.position.y);
            }
            else
            {
                movementAmount = movementSpeed * Time.deltaTime;
                transform.position = new Vector2(transform.position.x - movementAmount, transform.position.y);
            }
        }
        else
        {
            currentMovementTime = 0;
            movementAmount = 0;
            transform.position = spawnPosition;
            hasHitPlayer = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Platform") || other.CompareTag("Wall"))
        {
            hasHitPlayer = true;
        }
    }
}
