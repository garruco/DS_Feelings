using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public GameObject playerObj;
        private float defaultMoveSpeed;
        public float moveSpeed;
        public float speedMultiplier;
        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        bool readyToJump = true;

        bool movementEnabled = true;

        [Header("Noise")]

        public NoiseManager noiseManager;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;

        float horizontalInput;
        float verticalInput;

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;
        bool grounded;
        public AudioManager audioManager;

        //public Transform orientation;

        Vector3 moveDirection;

        Rigidbody rb;
        private void Awake()
        {
            defaultMoveSpeed = moveSpeed;
            rb = playerObj.GetComponent<Rigidbody>();
            //rb.freezeRotation = true;
        }

        private void Update()
        {
            grounded = Physics.Raycast(transform.position, Vector2.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            if (movementEnabled)
            {
                MyInput();
                SpeedControl();
            }
            else
            {
                StopPlayer();
            } 

            if (grounded)
                rb.drag = groundDrag;
            else rb.drag = 0;

        }
        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MyInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            float arousal = (noiseManager.GetArousalEdited() + 1f) * 0.35f + 0.2f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = defaultMoveSpeed * speedMultiplier * arousal;
            }
            else moveSpeed = defaultMoveSpeed * arousal;

            if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void MovePlayer()
        {
            moveDirection = Vector3.forward * verticalInput + Vector3.right * horizontalInput;
            //moveDirection = oritentation.forward * verticalInput + oritentation.right * horizontalInput;

            //Vector3 rotationAxis = moveDirection + Vector3.right;
            Vector3 rotationAxis = Quaternion.Euler(0, 90, 0) * moveDirection;
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float flatVelMagniturde = flatVel.magnitude;

            //playerObj.transform.Rotate(rotationAxis * flatVelMagniturde * rotationSpeed * Time.deltaTime, Space.Self); //TODO fix

            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 9f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 9f * airMultiplier, ForceMode.Force);
            }
        }

        private void StopPlayer()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float thisVelocity = flatVel.magnitude * 0.01f;
            Vector3 limitedVel = flatVel.normalized * thisVelocity;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {

                //Vector3 limitedVel = flatVel.normalized * moveSpeed;

                float diff = moveSpeed - flatVel.magnitude;
                float thisVelocity = flatVel.magnitude + diff * 0.01f;
                Vector3 limitedVel = flatVel.normalized * thisVelocity;

                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            float arousal = (noiseManager.GetArousalRaw() + 1) / 1.5f + 0.2f;
            float currentJumpForce = jumpForce * arousal;
            rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            audioManager.ChangeVolume("Jump", arousal / 3);
            audioManager.ChangePitch("Jump", arousal - 0.1f);
            audioManager.PlaySound("Jump");
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        public GameObject GetCubeStandingOn()
        {
            Ray downRay = new Ray(transform.position, -Vector3.up);
            RaycastHit toGround;

            bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

            GameObject hit = toGround.collider.gameObject;

            if(hit.name.Contains("ube")) return hit;

            Debug.LogWarning("no cube hit");
            
            return null;
        }

        public Vector2 GetPosFlat()
        {
            return new Vector2(rb.position.x, rb.position.z);
        }

        public float GetVelocity()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            return flatVel.magnitude;
        }

        public void DisableMovement()
        {
            movementEnabled = false;
        }

        public void EnableMovement()
        {
            movementEnabled = true;
        }
    }
}