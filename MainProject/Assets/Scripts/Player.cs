using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Vector3 velocity;
    public Vector3 cameraOffset;
    public bool isAttack;
    public Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = Camera.main.transform.position - transform.position;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity.x += -1f;
            velocity.z += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity.x += 1f;
            velocity.z += -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x += -1f;
            velocity.z += -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += 1f;
            velocity.z += 1f;
        }

        if(Input.GetMouseButton(0))
        {
            isAttack = true;
        }
        else
        {
            //애니메이션 결과 반영후 작업
            isAttack = false;
        }

        velocity.Normalize();

        if(!isAttack && velocity.magnitude > 0.1f)
        {
            Quaternion destLook = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, destLook, 7f * Time.deltaTime);

            body.MovePosition(transform.position + 3f * Time.deltaTime * velocity);
            transform.position += 3f * Time.deltaTime * velocity;
            GetComponentInChildren<Animator>().SetBool("IsMove", true);
            GetComponentInChildren<Animator>().SetBool("IsAttack", false);
        }
        else if(isAttack)
        {
            Quaternion destLook = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, destLook, 7f * Time.deltaTime);
            GetComponentInChildren<Animator>().SetBool("IsAttack", true);
            GetComponentInChildren<Animator>().SetBool("IsMove", false);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("IsAttack", false);
            GetComponentInChildren<Animator>().SetBool("IsMove", false);
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + cameraOffset, 5f * Time.deltaTime);
    }
}
