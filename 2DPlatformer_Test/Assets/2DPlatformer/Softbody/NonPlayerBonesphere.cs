namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;
    using UnityEngine.InputSystem;

    public class NonPlayerBonesphere : MonoBehaviour
    {
        [Header("Bones")]
        public GameObject root = null;
        public GameObject x = null;
        public GameObject x2 = null;
        public GameObject y = null;
        public GameObject y2 = null;
        public GameObject z = null;
        public GameObject z2 = null;
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

        private void Awake()
        {
            Softbody.Init(Shape, ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation, PrefabLine, ViewLines);

            Softbody.AddCollider(ref root, Softbody.ColliderShape.Sphere, 0.6f, 1f);
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
        }

        private void FixedUpdate()
        {
            root.transform.position = new Vector3(0, root.transform.position.y, root.transform.position.z);
        }
    }
}