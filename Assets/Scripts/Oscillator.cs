using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 MovementVector;
    [SerializeField] float TimePeriod = 2f;

    [Range(-1, 1)] [SerializeField] float MovementFactor;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimePeriod <= Mathf.Epsilon)
        {
            return;
        }
     
        var cycleOffset = Time.time / TimePeriod;
        var tau = Mathf.PI * 2;
        float oscillationFactor = Mathf.Sin(cycleOffset * tau);
        MovementFactor = oscillationFactor;//(oscillationFactor / 2.0f) + 0.5f;
        var offset = MovementVector * MovementFactor;

        transform.position = startPos + offset;
    }
}
