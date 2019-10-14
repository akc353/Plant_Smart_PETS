//========= Copyright 2016-2018, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Vive
{
    [AddComponentMenu("HTC/VIU/Object Grabber/Basic Grabbable", 0)]
    public class GrabbableInteraction : MonoBehaviour
    , IColliderEventDragStartHandler
    , IColliderEventDragFixedUpdateHandler
    , IColliderEventDragUpdateHandler
    , IColliderEventDragEndHandler
    {
        public bool grabbed;

        [Serializable]
        public class UnityEventGrabbable : UnityEvent<GrabbableInteraction> { }

        public const float MIN_FOLLOWING_DURATION = 0.02f;
        public const float DEFAULT_FOLLOWING_DURATION = 0.04f;
        public const float MAX_FOLLOWING_DURATION = 0.5f;

        private OrderedIndexedTable<ColliderButtonEventData, RigidPose> eventList = new OrderedIndexedTable<ColliderButtonEventData, RigidPose>();

        public bool alignPosition;
        public bool alignRotation;
        public Vector3 alignPositionOffset;
        public Vector3 alignRotationOffset;
        [Range(MIN_FOLLOWING_DURATION, MAX_FOLLOWING_DURATION)]
        public float followingDuration = DEFAULT_FOLLOWING_DURATION;
        public bool overrideMaxAngularVelocity = true;
        public bool unblockableGrab = true;

        [SerializeField]
        private ColliderButtonEventData.InputButton m_grabButton = ColliderButtonEventData.InputButton.Trigger;

        public UnityEventGrabbable afterGrabbed = new UnityEventGrabbable();
        public UnityEventGrabbable beforeRelease = new UnityEventGrabbable();
        public UnityEventGrabbable onDrop = new UnityEventGrabbable(); // change rigidbody drop velocity here

        private RigidPose m_prevPose = RigidPose.identity; // last frame world pose

        public ColliderButtonEventData.InputButton grabButton
        {
            get
            {
                return m_grabButton;
            }
            set
            {
                m_grabButton = value;
                // set all child MaterialChanger heighlightButton to value;
                var matChangers = ListPool<MaterialChanger>.Get();
                GetComponentsInChildren(matChangers);
                for (int i = matChangers.Count - 1; i >= 0; --i) { matChangers[i].heighlightButton = value; }
                ListPool<MaterialChanger>.Release(matChangers);
            }
        }

        public bool isGrabbed { get { return eventList.Count > 0; } }

        public ColliderButtonEventData grabbedEvent { get { return isGrabbed ? eventList.GetLastKey() : null; } }

        // effected rigidbody
        public Rigidbody rigid { get; set; }

        private bool moveByVelocity { get { return !unblockableGrab && rigid != null && !rigid.isKinematic; } }

        private RigidPose GetEventPose(ColliderButtonEventData eventData)
        {
            var grabberTransform = eventData.eventCaster.transform;
            return new RigidPose(grabberTransform);
        }
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            grabButton = m_grabButton;
        }
#endif

        protected virtual void Awake()
        {
            rigid = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            grabButton = m_grabButton;
        }

        protected virtual void OnDisable()
        {
            if (isGrabbed && beforeRelease != null)
            {
                beforeRelease.Invoke(this);
            }

            eventList.Clear();
        }

        public virtual void OnColliderEventDragStart(ColliderButtonEventData eventData)
        {
            grabbed = true;
        }

        public virtual void OnColliderEventDragFixedUpdate(ColliderButtonEventData eventData)
        {
        }

        public virtual void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
        {
        }

        public virtual void OnColliderEventDragEnd(ColliderButtonEventData eventData)
        {
            grabbed = false;
        }

        private void DoDrop()
        {
        }
    }
}