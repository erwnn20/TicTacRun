using UnityEngine;
using System.Collections;

public class CarControllerProto : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    void Start()
    {

    }

    void Update()
    {
        float moveVertical = Input.GetAxisRaw("Vertical");
        float rotation = Input.GetAxisRaw("Horizontal");

        transform.Translate(Vector3.forward * moveVertical * speed * Time.deltaTime);

        transform.Rotate(Vector3.left, rotation * rotationSpeed * Time.deltaTime);
    }
}
