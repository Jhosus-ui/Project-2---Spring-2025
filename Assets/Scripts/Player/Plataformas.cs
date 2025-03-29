using UnityEngine;

public class Plataformas : MonoBehaviour
{
    PlatformEffector2D pE2D;
    public bool LeftPlatform;
    public float upwardForce = 5f;
    private float dropTimer = 0f;
    private float dropDelay = 0.5f;

    void Start()
    {
        pE2D = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
 
        if (LeftPlatform && Time.time > dropTimer)
        {
            pE2D.rotationalOffset = 0;
            LeftPlatform = false;
            gameObject.layer = 6; 
        }

        if (Input.GetKeyDown(KeyCode.S) && !LeftPlatform)
        {
            pE2D.rotationalOffset = 180;
            LeftPlatform = true;
            gameObject.layer = 2; 

            dropTimer = Time.time + dropDelay;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !LeftPlatform)
        {
 
            if (collision.relativeVelocity.y > 0)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

                if (playerRb != null && playerMovement != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, upwardForce);
                    playerMovement.StartSubidita();
                }
            }
            else
            {
                pE2D.rotationalOffset = 0;
                LeftPlatform = false;
            }
        }
    }
}