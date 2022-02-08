namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;
    using UnityEngine.InputSystem;

    public class BoneSphere : MonoBehaviour
    {
        [Header("Bones")]
        public GameObject root = null;
        public GameObject x = null;
        public GameObject x2 = null;
        public GameObject y = null;
        public GameObject y2 = null;
        public GameObject z = null;
        public GameObject z2 = null;
        [SerializeField]
        private GameObject _bonesCenter = null;
        [SerializeField]
        private float _snapFactor = 1.5f;
        [SerializeField]
        private Timer _snapTimer;
        [SerializeField]
        private PlayerController _playerController = null;
        //public GameObject newBone = null;
        //public GameObject newBone2 = null;
        //public GameObject newBone3 = null;
        //public GameObject newBone4 = null;
        //public GameObject newBone5 = null;
        //public GameObject newBone6 = null;
        //public GameObject newBone7 = null;
        //public GameObject newBone8 = null;
        [Header("Spring Joint Settings")]
        [Tooltip("Strength of spring")]
        public float Spring = 100f;
        [Tooltip("Higher the value the faster the spring oscillation stops")]
        public float Damper = 0.2f;
        [Header("Other Settings")]
        public Softbody.ColliderShape Shape = Softbody.ColliderShape.Box;
        public float ColliderSize = 0.002f;
        public float RootColliderSize = 0.45f;
        public float RigidbodyMass = 1f;
        public LineRenderer PrefabLine = null;
        public bool ViewLines = true;


        private List<GameObject> _bones = new List<GameObject>();
        private List<SpringJoint> _springJoints = new List<SpringJoint>();
        private List<Vector3> _jointsAnchorsPositions = new List<Vector3>();
        private List<Vector3> _defaultAnchorsPositions = new List<Vector3>();
        private List<float> _jointsDistance = new List<float>();
        private List<float> _defaultBonesDistance = new List<float>();
        private float _currentScale = 1f;


        public List<GameObject> Bones => _bones;
        public List<SpringJoint> SpringJoints => _springJoints;
        public Timer SnapTimer => _snapTimer;

        private void Awake()
        {
            Softbody.Init(Shape, ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation, PrefabLine, ViewLines);

            Softbody.AddCollider(ref root, Softbody.ColliderShape.Sphere, 0.45f, 1f, true);
            Softbody.AddCollider(ref x, true);
            Softbody.AddCollider(ref x2, true);
            Softbody.AddCollider(ref y, true);
            Softbody.AddCollider(ref y2, true);
            Softbody.AddCollider(ref z, true);
            Softbody.AddCollider(ref z2, true);
            //Softbody.AddCollider(ref newBone);
            //Softbody.AddCollider(ref newBone2);
            //Softbody.AddCollider(ref newBone3);
            //Softbody.AddCollider(ref newBone4);
            //Softbody.AddCollider(ref newBone5);
            //Softbody.AddCollider(ref newBone6);
            //Softbody.AddCollider(ref newBone7);
            //Softbody.AddCollider(ref newBone8);

            Softbody.AddSpring(ref x, ref root);
            Softbody.AddSpring(ref x2, ref root);
            Softbody.AddSpring(ref y, ref root);
            Softbody.AddSpring(ref y2, ref root);
            Softbody.AddSpring(ref z, ref root);
            Softbody.AddSpring(ref z2, ref root);
            //Softbody.AddSpring(ref newBone, ref root);
            //Softbody.AddSpring(ref newBone2, ref root);
            //Softbody.AddSpring(ref newBone3, ref root);
            //Softbody.AddSpring(ref newBone4, ref root);
            //Softbody.AddSpring(ref newBone5, ref root);
            //Softbody.AddSpring(ref newBone6, ref root);
            //Softbody.AddSpring(ref newBone7, ref root);
            //Softbody.AddSpring(ref newBone8, ref root);
            _bones.Add(x);
            _bones.Add(x2);
            _bones.Add(y);
            _bones.Add(y2);
            _bones.Add(z);
            _bones.Add(z2);

            _springJoints.AddRange(GetComponentsInChildren<SpringJoint>());
        }

        private void OnEnable()
        {
            _snapTimer.StateChanged -= SnapBones;
            _snapTimer.StateChanged += SnapBones;
            _playerController.ResetPlayerPerformed -= ResetPlayer;
            _playerController.ResetPlayerPerformed += ResetPlayer;

            for (int i = 0; i < _bones.Count; i++)
            {
                _jointsAnchorsPositions.Add(_springJoints[i].connectedAnchor);
                _jointsDistance.Add(Vector3.Distance(_jointsAnchorsPositions[i], _bonesCenter.transform.localPosition));
            }
            _defaultAnchorsPositions.AddRange(_jointsAnchorsPositions);
            _defaultBonesDistance.AddRange(_jointsDistance);
        }

        private void OnDisable()
        {
            _snapTimer.StateChanged -= SnapBones;
            _playerController.ResetPlayerPerformed -= ResetPlayer;
        }

        private void FixedUpdate()
        {
            root.transform.position = new Vector3(0, root.transform.position.y, root.transform.position.z);
        }

        private void Update()
        {
            for (int i = 0; i < _bones.Count - 1; i++)
            {
                //Debug.Log("Current bone = " + _bones[i].name + " // " + "Default distance to root = " + _bonesDistance[i] + " // " + "Current distance to root = " + Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition));
                if ((Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition) * 100 > _jointsDistance[i] * _snapFactor || z.transform.position.y > z2.transform.position.y) && _snapTimer.IsRunning == false)
                {
                    _snapTimer.Start();
                }
            }
            if (_snapTimer.IsRunning == true)
            {
                _snapTimer.Update();
            }
        }

        private void SnapBones(Timer timer, Timer.State state)
        {
            for (int i = 0; i < _bones.Count - 1; i++)
            {
                if (Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition) * 100 > _jointsDistance[i] * _snapFactor)
                {
                    _bones[i].transform.localPosition = new Vector3(_jointsAnchorsPositions[i].x / 100, _jointsAnchorsPositions[i].y / 100, _jointsAnchorsPositions[i].z / 100);
                }
            }
        }

        public void ForceSnapBones()
        {
            for (int i = 1; i < _bones.Count; i++)
            {
                if (Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition) * 100 > _jointsDistance[i] * _snapFactor)
                {
                    _bones[i].transform.localPosition = new Vector3(_jointsAnchorsPositions[i].x / 100, _jointsAnchorsPositions[i].y / 100, _jointsAnchorsPositions[i].z / 100);
                }
            }
        }
         
        private void ResetPlayer(PlayerController sender, InputAction.CallbackContext obj)
        {
            LevelReferences.Instance.PlayerStart.ResetPlayerPosition();
            if (_snapTimer.IsRunning == false)
            {
                _snapTimer.Start();
            }
        }

        public void UpdateOnScaleChange(float scale)
        {
            _currentScale = scale;
            for (int i = 0; i < _bones.Count; i++)
            {
                _springJoints[i].autoConfigureConnectedAnchor = false;
                _springJoints[i].connectedAnchor = new Vector3(_defaultAnchorsPositions[i].x * scale, _defaultAnchorsPositions[i].y * scale, _defaultAnchorsPositions[i].z * scale);
                if (scale < 1)
                    _springJoints[i].spring = Spring * (1 + (1 - scale));
                else _springJoints[i].spring = Spring * (1 - scale);
            }

            _jointsAnchorsPositions.Clear();
            _jointsDistance.Clear();
            for (int i = 0; i < _bones.Count; i++)
            {
                _jointsAnchorsPositions.Add(_springJoints[i].connectedAnchor);
                _jointsDistance.Add(Vector3.Distance(_jointsAnchorsPositions[i], _bonesCenter.transform.localPosition));
            }
        }
    }
}