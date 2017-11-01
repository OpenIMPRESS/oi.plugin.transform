using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oi.plugin.transform {

    public class TransformCopy : MonoBehaviour {
        public int transformID;

        // Use this for initialization
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {

        }

        public void NewTransform(Vector3 pos, Quaternion rot) {
            transform.position = pos;
            transform.rotation = rot;
        }
    }

}