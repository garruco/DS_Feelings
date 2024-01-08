using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Artifact
{
    public class Diamond : MonoBehaviour
    {
        [Header("Measurements")]
        public float diamondHeight;
        public float minimumHeight;
        public float maximumHeight;
        private float gapHeight;
        private float timeTracker;

        [Header("Variables")]
        private Animator anim;
        private PlayerMovement playerMovement;
        private bool movingToPlayer = false;
        public LayerMask whatIsGround;
        private bool wasCollected = false;
        public AudioManager audioManager;

        void Awake()
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            timeTracker = Random.Range(0, 5);
            gapHeight = maximumHeight - minimumHeight;
            anim = GetComponent<Animator>();
            //anim.enabled = false;
            FixPosition();
        }

        // Update is called once per frame
        void Update()
        {
            Ray downRay = new Ray(transform.position, -Vector3.up);
            RaycastHit toGround;

            bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

            if (movingToPlayer)
            {
                MoveToPlayer();
                return;
            }

                        float targetHeight;

            if (onGround)
            {
                timeTracker += Time.deltaTime;
                float SinValue = Mathf.Sin(timeTracker * 1.5f) / 2 + 1;

                float hitPointY = toGround.point.y; //gets y coordinates of intersection of raycast and ground (cube);

                targetHeight = hitPointY + minimumHeight + diamondHeight / 2 + gapHeight * SinValue;


                gameObject.transform.Rotate(Vector3.up, 3);
            }
            else
            {
                targetHeight = 50;
            }

            float currentHeight = gameObject.transform.position.y + (targetHeight - gameObject.transform.position.y) * 0.02f;

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, currentHeight, gameObject.transform.position.z);
        }

        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player" && !wasCollected && !movingToPlayer)
            {
                Collected();
            }
        }

        private void Collected()
        {
            //anim.enabled = true;
            anim.Play("diamondCollected");
            audioManager.PlaySound("ApanharDiamante");
        }

        public void CollectionDone()
        {
            wasCollected = true;
        }

        private void FixPosition()
        {
            if (gameObject.transform.position.x % 2 != 0)
            {
                float newXPos = Mathf.Round(gameObject.transform.position.x);
                if (newXPos % 2 != 0)
                {
                    newXPos++;
                }
                gameObject.transform.position = new Vector3(newXPos, gameObject.transform.position.y, gameObject.transform.position.z);
            }
            if (gameObject.transform.position.z % 2 != 0)
            {
                float newZPos = Mathf.Round(gameObject.transform.position.z);
                if (newZPos % 2 != 0)
                {
                    newZPos++;
                }
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, newZPos);
            }
        }

        public void Reset(Vector3 newPos)
        {
            gameObject.transform.position = newPos;
            FixPosition();
            //anim.Play("diamondIdle");
            wasCollected = false;
            movingToPlayer = false;
        }
        public void StartMovingToPlayer()
        {
            movingToPlayer = true;
        }
        public void MoveToPlayer()
        {
            var step = 6 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerMovement.gameObject.transform.position, step);
        }
        public bool GetCollectionStatus()
        {
            return wasCollected;
        }

        public void SetPos(Vector3 newPos)
        {
            gameObject.transform.position = newPos;
        }

        public Vector3 GetPos()
        {
            return transform.position;
        }

        public Vector2 GetPosFlat()
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }
}