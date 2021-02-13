using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    AudioClip thrustClip;

    [SerializeField]
    AudioClip deathClip;

    [SerializeField]
    AudioClip loadClip;

	[SerializeField]
	ParticleSystem thrustSystem;

	[SerializeField]
	ParticleSystem deathSystem;

	[SerializeField]
	ParticleSystem successSystem;


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
                InvokeDeathSequence();
                break;
        }
	}

    private void InvokeSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(loadClip);
		successSystem.Play();

        Invoke("LoadNextScene", 1f);
    }

    private void InvokeDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathClip);
		deathSystem.Play();
		
        Invoke("ResetGame", 1f);
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();

            CheckReset();
        } 
    }

    private void RespondToThrustInput()
    {
        if (state != State.Alive)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
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

		thrustSystem.Play();
    }

    void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

        var thrust = RCSThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(thrust * Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(thrust * -Vector3.forward);
        }

        rigidBody.freezeRotation = false;
    }

    private void CheckReset()
    {
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }
}
