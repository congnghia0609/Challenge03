using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public bool gameOver = false;

    public float floatForce = 10.0f;
    public float topBoundary = 14.5f;
    private bool isLowEnough = false;
    public bool isOnGround = false;
    private float gravityModifier = 1.5f;
    private static bool gravityInitialized = false;
    private Rigidbody playerRb;

    public ParticleSystem explosionParticle;
    public ParticleSystem fireworksParticle;

    private AudioSource playerAudio;
    public AudioClip moneySound;
    public AudioClip explodeSound;
    public AudioClip groundSound;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        // Dùng biến tĩnh static bool để giữ gravityInitialized tồn tại qua các lần tải lại scene.
        if (!gravityInitialized)
        {
            Physics.gravity *= gravityModifier;
            gravityInitialized = true;
        }
        gameManager = GameObject.Find("ReplayButton").GetComponent<GameManager>();
        gameManager.gameObject.SetActive(false);
        playerAudio = GetComponent<AudioSource>();

        playerRb = GetComponent<Rigidbody>();
        // Apply a small upward force at the start of the game
        playerRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // While space is pressed and player is low enough, float up
        if (Input.GetKeyDown(KeyCode.Space) && !isLowEnough && !gameOver)
        {
            playerRb.AddForce(Vector3.up * floatForce, ForceMode.Impulse);
        }
        if (isOnGround)
        {
            playerRb.AddForce(Vector3.up * floatForce, ForceMode.Impulse);
            isOnGround = false;
        }
        if (transform.position.y >= topBoundary)
        {
            isLowEnough = true;
            transform.position = new Vector3(transform.position.x, topBoundary, transform.position.z);
        }
        else
        {
            isLowEnough = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // if player collides with bomb, explode and set gameOver to true
        if (other.gameObject.CompareTag("Bomb"))
        {
            explosionParticle.Play();
            playerAudio.PlayOneShot(explodeSound, 1.0f);
            gameOver = true;
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
            Destroy(gameObject, 1.0f);
            gameManager.GameOver();
        }

        // if player collides with money, fireworks
        else if (other.gameObject.CompareTag("Money"))
        {
            fireworksParticle.Play();
            playerAudio.PlayOneShot(moneySound, 1.0f);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            if (!gameOver)
            {
                playerAudio.PlayOneShot(groundSound, 1.0f);
            }
        }
    }
}
