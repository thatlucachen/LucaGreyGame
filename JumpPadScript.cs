using System.Collections;
using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    private Animator jumpPadAnimation;
    float jumpPadAnimationTime = 0.2f;

    private void Awake()
    {
        jumpPadAnimation = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(JumpPadAnimationTimer());
        }
    }

    // Jump pad animation timer
    IEnumerator JumpPadAnimationTimer()
    {
        jumpPadAnimation.SetBool("jumpPadOn", true);
        yield return new WaitForSeconds(jumpPadAnimationTime);
        jumpPadAnimation.SetBool("jumpPadOn", false);
    }
}
