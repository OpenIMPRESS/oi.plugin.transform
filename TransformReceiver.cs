using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oi.core.network;
using System.IO;

namespace oi.plugin.transform {

    public class TransformReceiver : MonoBehaviour {
        private Dictionary<int,TransformCopy> transformCopies = new Dictionary<int, TransformCopy>();
        public UDPConnector connector;

        // Use this for initialization
        void Start() {
            TransformCopy[] _transformCopies = FindObjectsOfType<TransformCopy>();

            foreach(TransformCopy transformCopy in _transformCopies) {
                transformCopies.Add(transformCopy.transformID, transformCopy);
            }
        }

        // Update is called once per frame
        void Update() {
            ParseData(connector);
        }

        private void ParseData(UDPConnector udpSource) {
            byte[] data = udpSource.GetNewData();

            while (data != null) {
                // Make sure there is data in the stream.
                if (data.Length != 0) {

                    int packetID = -1;
                    using (MemoryStream str = new MemoryStream(data)) {
                        using (BinaryReader reader = new BinaryReader(str)) {
                            packetID = reader.ReadInt32();
                        }
                    }

                    if (packetID == 2) { // transform packet
                        Vector3 pos;
                        Quaternion rot;
                        int transformID;
                        TransformSerializer.Deserialize(data, out transformID, out pos, out rot);

                        if (transformCopies.ContainsKey(transformID)) {
                            transformCopies[transformID].NewTransform(pos, rot);
                        }
                    }
                }

                data = udpSource.GetNewData();
            }
        }

    }

}