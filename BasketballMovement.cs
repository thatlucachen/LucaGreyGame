// This script controls the movement of basketballs in the game.

using UnityEngine;

public class BasketballMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    float movementAmount = 22;
    bool isMovingDown = true;
    float maxHeight = 25;
    float minHeight = 15f;

    void Update()
    {
        if (isMovingDown)
        {
            movementAmount += -3 * Time.deltaTime;
        }
        else
        {
            movementAmount += 6 * Time.deltaTime;
        }

        if (rigidBody.position.y <= minHeight)
        {
            isMovingDown = false;
        }
        if (rigidBody.position.y >= maxHeight)
        {
            isMovingDown = true;
        }

        transform.position = new Vector2(179.11f, movementAmount);
    }
}
