/*
This file is part of the OpenIMPRESS project.

OpenIMPRESS is free software: you can redistribute it and/or modify
it under the terms of the Lesser GNU Lesser General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

OpenIMPRESS is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with OpenIMPRESS. If not, see <https://www.gnu.org/licenses/>.
*/

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
            OIMSG msg = udpSource.GetNewData();

            while (msg != null && msg.data != null && msg.data.Length > 0) {
                int packetID = -1;
                using (MemoryStream str = new MemoryStream(msg.data)) {
                    using (BinaryReader reader = new BinaryReader(str)) {
                        packetID = reader.ReadInt32();
                    }
                }

                if (packetID == 2) { // transform packet
                    Vector3 pos;
                    Quaternion rot;
                    int transformID;
                    TransformSerializer.Deserialize(msg.data, out transformID, out pos, out rot);

                    if (transformCopies.ContainsKey(transformID)) {
                        transformCopies[transformID].NewTransform(pos, rot);
                    }
                }
                msg = udpSource.GetNewData();
            }
        }

    }

}