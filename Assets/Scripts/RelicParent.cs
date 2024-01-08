using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Noise;

namespace Artifact
{
    public class RelicParent : MonoBehaviour
    {
        [Header("Measurements")]
        public float relicHeight;
        public float minimumHeight;
        public float maximumHeight;
        private float gapHeight;
        private float timeTracker;

        [Header("Variables")]
        public GameObject childRelic;
        private Animator anim;
        private PlayerMovement playerMovement;
        public NoiseManager noiseManager;
        public LayerMask whatIsGround;
        private bool wasCollected = false;
        public AudioManager audioManager;

        private float previousArousal;
        private float previousValence;
        void Start()
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            timeTracker = Random.Range(0, 5);
            gapHeight = maximumHeight - minimumHeight;
            anim = childRelic.GetComponent<Animator>();
            anim.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            Ray downRay = new Ray(transform.position, -Vector3.up);
            RaycastHit toGround;

            bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

            if (onGround)
            {
                timeTracker += Time.deltaTime;
                float SinValue = Mathf.Sin(timeTracker * 1.5f) / 2 + 1;

                float hitPointY = toGround.point.y; //gets y coordinates of intersection of raycast and ground (cube);

                float targetHeight = hitPointY + minimumHeight + relicHeight / 2 + gapHeight * SinValue;
                float currentHeight = gameObject.transform.position.y + (targetHeight - gameObject.transform.position.y) * 0.02f;

                gameObject.transform.position = new Vector3(gameObject.transform.position.x, currentHeight, gameObject.transform.position.z);

                float arousalDiff = noiseManager.GetArousalRaw() - previousArousal;
                float valenceDiff = noiseManager.GetValenceRaw() - previousValence;

                gameObject.transform.Rotate(Vector3.up, arousalDiff * Random.Range(-5000, 10000));
                //gameObject.transform.Rotate(Vector3.left, valenceDiff * Random.Range(5000, 10000));

                //if(transform.TransformDirection(Vector3.up).y < 0 && !anim.enabled) gameObject.transform.Rotate(Vector3.left, 180); //check if working

                previousArousal = noiseManager.GetArousalRaw();
                previousValence = noiseManager.GetValenceRaw();
            }
            else
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 50, gameObject.transform.position.z);
            }
        }

        public void OnTrigger(Collider collision)
        {
            if (collision.gameObject.tag == "Player" && !wasCollected && !anim.enabled)
            {
                Collected();
            }
        }

        void Collected()
        {
            anim.enabled = true;
            anim.Play("relicReached");
        }

        public void StartCollection()
        {

        }

        public void CollectionDone()
        {
            Debug.Log("WAS COLLECTED");
            audioManager.PlaySound("ApanharReliquia");
            wasCollected = true;
            Invoke(nameof(DisableAnim), 3);
        }

        private void DisableAnim()
        {
            anim.enabled = false;
        }

        private void OnEnable() {
            childRelic.transform.position = gameObject.transform.position;
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
            wasCollected = false;
        }

        public Vector2 GetPosFlat(){
            return new Vector2(transform.position.x, transform.position.z);
        }

        public bool GetCollectionStatus(){
            return wasCollected;
        }

        public void SetCollectionFalse(){
            wasCollected = false;
        }
    }
}
