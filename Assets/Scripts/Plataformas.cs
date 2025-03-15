using UnityEngine;

public class Plataformas : MonoBehaviour
{

    PlatformEffector2D pE2D;

    public bool LeftPlatform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pE2D = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !LeftPlatform)
        {
            pE2D.rotationalOffset = 180;

            LeftPlatform = true;

            gameObject.layer = 2;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        pE2D.rotationalOffset = 0;

        LeftPlatform = false;

        gameObject.layer = 6 & 7;
    }
}
