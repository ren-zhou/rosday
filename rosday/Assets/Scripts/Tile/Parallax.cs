using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 prevPos;

    [SerializeField] private Vector2 multiplier;

    void Start()
    {
        camTransform = Camera.main.transform;
        prevPos = camTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = camTransform.position - prevPos;
        transform.position += new Vector3(delta.x * multiplier.x, delta.y * multiplier.y);
        prevPos = camTransform.position;
    }
}
