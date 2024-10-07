using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public Animator animator;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public bool jumpWait = false;
 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
 
    Vector3 velocity;
 
    bool isGrounded;

    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    public bool isMoving;
 
    // Update is called once per frame
    void Update()
    {
        if(!DialogSystem.Instance.dialogUIActive && !StorageManager.Instance.storageUIOpen && !CampfireUIManager.Instance.isUiOpen)
        {
        Movement();
        }
    }

    public void Movement()
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
 
        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;
 
        controller.Move(move * speed * Time.deltaTime);
 
        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded && !jumpWait)
        {
            jumpWait = true;
            PlayerState.Instance.currentCalories -= 10;
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StartCoroutine(NewJumpDelay());
        }
 
        velocity.y += gravity * Time.deltaTime;
 
        controller.Move(velocity * Time.deltaTime);

        if(lastPosition != gameObject.transform.position && isGrounded == true)
        {
            animator.SetBool("isRunning", true);
            isMoving = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.grassWalkSound);
            SoundManager.Instance.woodWalkSound.Stop();
            if(WeatherSystem.Instance.isSpecialWeather)
            {
                WeatherSystem.Instance.rainEffect.SetActive(true);
            }
        }
        else if(lastPosition != gameObject.transform.position && isGrounded == false && !jumpWait)
        {
            isMoving = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.woodWalkSound);
            SoundManager.Instance.grassWalkSound.Stop();
            if(WeatherSystem.Instance.isSpecialWeather)
            {
                WeatherSystem.Instance.rainEffect.SetActive(false);
            }
        }
        else
        {
            isMoving = false;
            SoundManager.Instance.grassWalkSound.Stop();
            SoundManager.Instance.woodWalkSound.Stop();
            animator.SetBool("isRunning", false);
        }
        lastPosition = gameObject.transform.position;
    }

    IEnumerator NewJumpDelay()
    {
        yield return new WaitForSeconds(0.9f);
        jumpWait = false;
    }
}
