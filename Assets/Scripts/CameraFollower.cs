using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField]
    public GameObject Subject;
    
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = Subject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var position = Subject.transform.position;
        var diff = position - lastPosition;
        lastPosition = position;

        transform.position = transform.position + diff;
        transform.LookAt(Subject.transform, Vector3.up);
    }
}
