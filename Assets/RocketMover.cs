using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

	void OnCollisionEnter(Collision collision) {
		switch(collision.gameObject.tag) {
			case "Friendly":
				print("OK");
				break;
			default:
				print("Dead");
				break;
		}
	}

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();

        CheckReset();
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
