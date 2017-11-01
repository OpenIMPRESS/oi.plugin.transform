using SysDiag = System.Diagnostics;
using System.IO;
using UnityEngine;

namespace oi.plugin.transformsend {

    public static class TransformSerializer {
        /// <summary>
        /// The trans header consists of one 32 bit integers.
        /// </summary>
        private static int HeaderSize = sizeof(int) * 1;


        public static byte[] Serialize(Transform trans, int id) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    WriteTrans(writer, trans.position, trans.rotation, id);

                    stream.Position = 0;
                    data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                }
            }

            return data;
        }

        public static byte[] Serialize(Vector3 pos, Quaternion rot, int id) {
            byte[] data = null;

            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    WriteTrans(writer, pos, rot, id);

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
                    while (reader.BaseStream.Length - reader.BaseStream.Position >= HeaderSize) {
                        ReadTrans(reader, out transformID, out pos, out rot);
                    }
                }
            }
        }


        private static void WriteTrans(BinaryWriter writer, Vector3 pos, Quaternion rot, int id) {
            SysDiag.Debug.Assert(writer != null);

            WriteTransHeader(writer, id);
            WritePosition(writer, pos);
            WriteRotation(writer, rot);
        }


        private static void ReadTrans(BinaryReader reader, out int transformID, out Vector3 pos, out Quaternion rot) {
            SysDiag.Debug.Assert(reader != null);

            int packetID = -1;
            transformID = -1;

            pos = new Vector3();
            rot = new Quaternion();

            ReadHeader(reader, out packetID, out transformID);
            if (packetID != 2) return;

            pos = ReadPosition(reader);
            rot = ReadRotation(reader);
        }


        private static void WriteTransHeader(BinaryWriter writer, int id) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(2); // '2' announces transform packet
            writer.Write(id);
        }


        private static void ReadHeader(BinaryReader reader, out int packetID, out int transformID) {
            SysDiag.Debug.Assert(reader != null);

            packetID = reader.ReadInt32();
            transformID = reader.ReadInt32();
        }


        private static void WritePosition(BinaryWriter writer, Vector3 location) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(location.x);
            writer.Write(location.y);
            writer.Write(location.z);
        }


        private static Vector3 ReadPosition(BinaryReader reader) {
            SysDiag.Debug.Assert(reader != null);

            Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return position;
        }

 
        private static void WriteRotation(BinaryWriter writer, Quaternion rotation) {
            SysDiag.Debug.Assert(writer != null);

            writer.Write(rotation.x);
            writer.Write(rotation.y);
            writer.Write(rotation.z);
            writer.Write(rotation.w);
        }


        private static Quaternion ReadRotation(BinaryReader reader) {
            SysDiag.Debug.Assert(reader != null);

            Quaternion rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return rotation;
        }
    }

}