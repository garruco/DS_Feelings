using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Noise;
using Player;

namespace Camera
{
    public class CameraManager : MonoBehaviour

    {
        public NoiseManager noiseManager;

        public float minOffsetY;
        public float maxOffsetY;
        private float yOffsetGap;

        public float minDistanceToPlayer;
        public float maxDistanceToPlayer;
        private float distanceToPlayerGap;
        private float lastDistanceToPlayer;

        public PlayerMovement playerMovement;
        public float maxPlayerSpeed = 8;

        private float lookAngle;

        private CinemachineVirtualCamera vcam;
        private CinemachineTransposer transposer;


        // TO-DO camera rotation with controller - how does it work with movement?
        void Start()
        {
            vcam = gameObject.GetComponent<CinemachineVirtualCamera>();
            transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();

            yOffsetGap = maxOffsetY - minOffsetY;
            distanceToPlayerGap = maxDistanceToPlayer - minDistanceToPlayer;
        }

        void Update()
        {
            float valence = noiseManager.GetValenceEdited();
            float arousal = ( noiseManager.GetArousalEdited() +1 ) / 2;

            float currentOffsetY = minOffsetY + arousal * yOffsetGap;

            //calculates and smoothens distance
            float targetDistance = -Mathf.Clamp(minDistanceToPlayer + (playerMovement.GetVelocity() / maxPlayerSpeed) * distanceToPlayerGap, minDistanceToPlayer, maxDistanceToPlayer);
            float currentDistance = lastDistanceToPlayer + (targetDistance - lastDistanceToPlayer) * 0.02f;
            lastDistanceToPlayer = currentDistance;

            Vector3 currentOffset = new Vector3(0, currentOffsetY, currentDistance);

            transposer.m_FollowOffset = currentOffset;
        }
    }
}
