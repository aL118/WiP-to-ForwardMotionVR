using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float baseSpeed = 5;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Terrain root;
    public Terrain midland1;
    public Terrain midland2;
    public Terrain midland3;
    public Terrain midland4;
    public GameObject faraway;

    Vector3 velocity;
    bool isGrounded;
    float speed = 5;
    int cycle = 0;
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey("left shift") && isGrounded)
        {
            speed = 10;
        }
        else
        {
            speed = baseSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if(cycle < (int)transform.position.z/480){
            cycle = (int)transform.position.z/480;
            if(cycle>1){
                if(cycle%3==1){
                    midland4.transform.position = new Vector3(-250,0,(cycle+1)*480+380);
                }else if(cycle%3==2){
                    midland1.transform.position = new Vector3(-250,0,(cycle+1)*480+380);
                }else if(cycle%3==3){
                    midland2.transform.position = new Vector3(-250,0,(cycle+1)*480+380);
                }else {
                    midland3.transform.position = new Vector3(-250,0,(cycle+1)*480+380);
                }
                faraway.transform.position = new Vector3(-250,0,(cycle+4)*480);
            }
            // terrain disappears after 1420
            Debug.Log(midland1.transform.position.z+" "+midland2.transform.position.z+" "+midland3.transform.position.z+" "+midland4.transform.position.z);
        }
    }
}