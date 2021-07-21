using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RocketMover : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    Vector3 originalPosition;
    Quaternion originalRotation;

    [SerializeField]
    int RCSThrust = 10;

    [SerializeField]
    int MainThrust = 100;

    [SerializeField] 
    Text collisionText;

    [SerializeField]
    AudioClip thrustClip;

    [SerializeField]
    AudioClip deathClip;

    [SerializeField]
    AudioClip loadClip;

	[SerializeField]
	ParticleSystem thrustSystem;

    [SerializeField]
    ParticleSystem leftThrustSystem;

    [SerializeField]
    ParticleSystem rightThrustSystem;

    [SerializeField]
	ParticleSystem deathSystem;

	[SerializeField]
	ParticleSystem successSystem;

    [SerializeField]
    float LoadLevelDelay = 2f;
    
    private bool collisionsEnabled = true;

    enum State
    {
        Transcending,
        Dying,
        Alive
    }

    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

	void OnCollisionEnter(Collision collision) {
        if (state != State.Alive)
        {
            return;
        }

 		switch(collision.gameObject.tag) {
			case "Friendly":
				break;
            case "Finnish":
                InvokeSuccessSequence();
                break;
            default:
                if (collisionsEnabled)
                {
                    InvokeDeathSequence();
                }

                break;
        }
	}

    private void InvokeSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(loadClip);
		successSystem.Play();

        Invoke("LoadNextScene", LoadLevelDelay);
    }

    private void InvokeDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathClip);
		deathSystem.Play();
		
        Invoke("ResetGame", LoadLevelDelay);
    }

    private void ResetGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentSceneIndex += 1;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            currentSceneIndex = 0;
        }
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();

            CheckDebug();
        } 
    }

    private void RespondToThrustInput()
    {
        if (state != State.Alive)
        {
			StopThrust();
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
			StopThrust();
        }
    }

	private void StopThrust() {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (thrustSystem.isPlaying)
        {
            thrustSystem.Stop();
        }
	}

    private void ApplyThrust()
    {
        var thrust = MainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrust);

        if (!audioSource.isPlaying && thrustClip != null)
        {
            audioSource.PlayOneShot(thrustClip);
        }

        if (thrustSystem.isStopped)
        {
            thrustSystem.Play();
        }
    }

    private void RotateRight(float thrust)
    {
        transform.Rotate(thrust * Vector3.forward);
        if (rightThrustSystem.isStopped)
        {
            rightThrustSystem.Play();
        }
    }
    
    private void RotateLeft(float thrust)
    {
        transform.Rotate(thrust * -Vector3.forward);

        if (leftThrustSystem.isStopped)
        {
            leftThrustSystem.Play();
        }
    }

    private void StopRotate()
    {
        if (rightThrustSystem.isPlaying)
        {
            rightThrustSystem.Stop();
        }

        if (leftThrustSystem.isPlaying)
        {
            leftThrustSystem.Stop();
        }
    }

    void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

        var thrust = RCSThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            RotateRight(thrust);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateLeft(thrust);
        }
        else
        {
            StopRotate();
        }

        rigidBody.freezeRotation = false;
    }

    private void CheckDebug()
    {
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.L))
        {
            InvokeSuccessSequence();
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
            if (collisionText != null)
            {
                collisionText.text = $"Collisions Enabled: {collisionsEnabled}";
            }
        }
    }
}
