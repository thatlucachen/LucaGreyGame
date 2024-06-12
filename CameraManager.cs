// This script controls the camera in the game, allowing it to track what room the player is in and go to that room.

using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject virtualCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(false);
        }
    }
}
