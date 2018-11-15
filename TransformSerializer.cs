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

using SysDiag = System.Diagnostics;
using System.IO;
using UnityEngine;

namespace oi.plugin.transform {

    public static class TransformSerializer {


        public static byte[] Serialize(Transform trans, int id) {
            return Serialize(trans.position, trans.rotation, id);
        }

        public static byte[] Serialize(Vector3 pos, Quaternion rot, int id) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {

                    writer.Write(2); // '2' announces transform packet
                    writer.Write(id);

                    WriteTrans(writer, pos, rot);

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }


        public static void Deserialize(byte[] data, out int transformID, out Vector3 pos, out Quaternion rot) {
            pos = new Vector3();
            rot = new Quaternion();
            transformID = -1;

            using (MemoryStream stream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    int packetID = reader.ReadInt32();
                    if (packetID != 2) return;

                    transformID = reader.ReadInt32();
                    ReadTrans(reader, out pos, out rot);
                }
            }
        }


        private static void WriteTrans(BinaryWriter writer, Vector3 pos, Quaternion rot) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(pos.x);
            writer.Write(pos.y);
            writer.Write(pos.z);

            writer.Write(rot.x);
            writer.Write(rot.y);
            writer.Write(rot.z);
            writer.Write(rot.w);
        }


        private static void ReadTrans(BinaryReader reader, out Vector3 pos, out Quaternion rot) {
            SysDiag.Debug.Assert(reader != null);

            pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            rot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

    }
}