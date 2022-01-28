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
        /*public GameObject newBone = null;
        public GameObject newBone2 = null;
        public GameObject newBone3 = null;
        public GameObject newBone4 = null;
        public GameObject newBone5 = null;
        public GameObject newBone6 = null;
        public GameObject newBone7 = null;
        public GameObject newBone8 = null;*/
        [Header("Spring Joint Settings")]
        [Tooltip("Strength of spring")]
        public float Spring = 100f;
        [Tooltip("Higher the value the faster the spring oscillation stops")]
        public float Damper = 0.2f;
        [Header("Other Settings")]
        public Softbody.ColliderShape Shape = Softbody.ColliderShape.Box;
        public float ColliderSize = 0.002f;
        public float RigidbodyMass = 1f;
        public LineRenderer PrefabLine = null;
        public bool ViewLines = true;

        private GameObject[] _bones = new GameObject[6];
        private Vector3[] _bonesPositions = new Vector3[6];
        private Vector3[] _defaultBonesPositions = new Vector3[6];
        private float[] _bonesDistance = new float[6];
        private float[] _defaultBonesDistance = new float[6];
        private float _currentScale = 1f;

        public GameObject[] Bones => _bones;

        private void Awake()
        {
            Softbody.Init(Shape, ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation, PrefabLine, ViewLines);

            Softbody.AddCollider(ref root, Softbody.ColliderShape.Sphere, 0.45f, 1f);
            Softbody.AddCollider(ref x);
            Softbody.AddCollider(ref x2);
            Softbody.AddCollider(ref y);
            Softbody.AddCollider(ref y2);
            Softbody.AddUndersideCollider(ref z);
            Softbody.AddCollider(ref z2);
            /*Softbody.AddCollider(ref newBone);
            Softbody.AddCollider(ref newBone2);
            Softbody.AddCollider(ref newBone3);
            Softbody.AddCollider(ref newBone4);
            Softbody.AddCollider(ref newBone5);
            Softbody.AddCollider(ref newBone6);
            Softbody.AddCollider(ref newBone7);
            Softbody.AddCollider(ref newBone8);*/

            Softbody.AddSpring(ref x, ref root);
            Softbody.AddSpring(ref x2, ref root);
            Softbody.AddSpring(ref y, ref root);
            Softbody.AddSpring(ref y2, ref root);
            Softbody.AddUndersideSpring(ref z, ref root);
            Softbody.AddSpring(ref z2, ref root);
            /*Softbody.AddSpring(ref newBone, ref root);
            Softbody.AddSpring(ref newBone2, ref root);
            Softbody.AddSpring(ref newBone3, ref root);
            Softbody.AddSpring(ref newBone4, ref root);
            Softbody.AddSpring(ref newBone5, ref root);
            Softbody.AddSpring(ref newBone6, ref root);
            Softbody.AddSpring(ref newBone7, ref root);
            Softbody.AddSpring(ref newBone8, ref root);*/
            _bones.SetValue(x, 0);
            _bones.SetValue(x2, 1);
            _bones.SetValue(y, 2);
            _bones.SetValue(y2, 3);
            _bones.SetValue(z, 4);
            _bones.SetValue(z2, 5);

        }

        private void OnEnable()
        {
            _snapTimer.StateChanged -= SnapBones;
            _snapTimer.StateChanged += SnapBones;
            _playerController.ResetPlayerPerformed -= ResetPlayer;
            _playerController.ResetPlayerPerformed += ResetPlayer;
            for (int i = 0; i < _bones.Length; i++)
            {
                _bonesPositions.SetValue(_bones[i].transform.localPosition, i);
                _bonesDistance.SetValue(Vector3.Distance(_bonesPositions[i], _bonesCenter.transform.localPosition), i);
            }
            _defaultBonesPositions = _bonesPositions;
            _defaultBonesDistance = _bonesDistance;
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
            //for (int i = 0; i < _bones.Length; i++)
            //{
            //    //Debug.Log("Current bone = " + _bones[i].name + " // " + "Default distance to root = " + _bonesDistance[i] + " // " + "Current distance to root = " + Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition));
            //    if ((Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition) > _bonesDistance[i] * _snapFactor || z.transform.position.y > z2.transform.position.y) && _snapTimer.IsRunning == false)
            //    {
            //        _snapTimer.Start();
            //    }
            //}
            //if (_snapTimer.IsRunning == true)
            //{
            //    _snapTimer.Update();
            //}
        }

        private void SnapBones(Timer timer, Timer.State state)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                if (Vector3.Distance(_bones[i].transform.localPosition, _bonesCenter.transform.localPosition) > _bonesDistance[i] * _snapFactor)
                {
                    _bones[i].transform.localPosition = _bonesPositions[i];
                }
            }
            if (z.transform.position.y > z2.transform.position.y)
            {
                _bones[4].transform.localPosition = _bonesPositions[4];
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
            for (int i = 0; i < _bonesPositions.Length; i++)
            {
                _bonesPositions[i] = new Vector3(_defaultBonesPositions[i].x * _currentScale, _defaultBonesPositions[i].y * _currentScale, _defaultBonesPositions[i].z * _currentScale);
                _bonesDistance.SetValue(Vector3.Distance(_bonesPositions[i], _bonesCenter.transform.localPosition), i);
            }
        }
    }
}