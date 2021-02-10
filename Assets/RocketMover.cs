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
                state = State.Transcending;
                Invoke("LoadNextScene", 1f);
                break;
            default:
                state = State.Dying;
                Invoke("ResetGame", 1f);
                break;
        }
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
            Thrust();
            Rotate();

            CheckReset();
        } else
        {
            audioSource.Stop();
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            var thrust = MainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrust);

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void Rotate()
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
