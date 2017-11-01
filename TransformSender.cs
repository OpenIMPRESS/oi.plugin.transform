using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oi.core.network;

namespace oi.plugin.transform {

    public class TransformSender : MonoBehaviour {
        public bool continuousSend = true;
        private bool continuousSendMeasure = false;
        private bool measuring = false;

        public float sendInterval = 0.01f;
        private float timer = 0;
        public int transformID;
        public float measureTime = 2;

        private List<Vector3> positions = new List<Vector3>();
        private List<Quaternion> rotations = new List<Quaternion>();

        private Vector3 position = new Vector3(0, 0, 0);
        private Quaternion rotation = new Quaternion(0, 0, 0, 0);

        public UDPConnector connector;

        void Update() {
            timer += Time.deltaTime;

            if (measuring) {
                continuousSendMeasure = false;
                positions.Add(transform.position);
                rotations.Add(transform.rotation);

                if (timer >= measureTime) {
                    measuring = false;
                    continuousSendMeasure = true;

                    position = FilteredMeanPos(positions);
                    rotation = FilteredMeanRot(rotations);
                }
            }

            if (continuousSend) {
                if (timer >= sendInterval) {
                    timer = 0;
                    byte[] serialized = TransformSerializer.Serialize(transform, transformID);
                    connector.SendData(serialized);
                }
            }

            if (continuousSendMeasure) {
                if (timer >= sendInterval) {
                    timer = 0;
                    byte[] serialized = TransformSerializer.Serialize(position, rotation, transformID);
                    connector.SendData(serialized);
                }
            }

        }

        public bool IsMeasuring() {
            return measuring;
        }

        public void MeasureAndSend() {
            continuousSend = false;
            measuring = true;
            timer = 0;
            positions.Clear();
            rotations.Clear();
            position = new Vector3(0, 0, 0);
            rotation = new Quaternion(0, 0, 0, 0);
        }


        public Vector3 FilteredMeanPos(List<Vector3> poss) {
            Vector3 mean = AvgPos(poss);
            float stdev = StdevPos(poss);
            List<Vector3> filtPoss = new List<Vector3>();

            foreach (Vector3 pos in poss) {
                float dist = Mathf.Abs(Vector3.Distance(mean, pos));
                if (dist < stdev) filtPoss.Add(pos);
            }

            return AvgPos(filtPoss);
        }

        public Quaternion FilteredMeanRot(List<Quaternion> rots) {
            Quaternion mean = AvgRot(rots);
            float stdev = StdevRot(rots);
            List<Quaternion> filtRots = new List<Quaternion>();

            foreach (Quaternion rot in rots) {
                float dist = Mathf.Abs(Quaternion.Angle(mean, rot));
                if (dist < stdev) filtRots.Add(rot);
            }

            return AvgRot(filtRots);
        }



        float StdevRot(List<Quaternion> rots) {
            Quaternion mean = AvgRot(rots);

            float variance = 0;
            foreach (Quaternion rot in rots) {
                variance += Mathf.Pow(Quaternion.Angle(mean, rot), 2);
            }
            variance /= rots.Count;
            return Mathf.Sqrt(variance);
        }

        float StdevPos(List<Vector3> poss) {
            Vector3 mean = AvgPos(poss);

            float variance = 0;
            foreach (Vector3 pos in poss) {
                variance += Mathf.Pow(Vector3.Distance(mean, pos), 2);
            }
            variance /= poss.Count;
            return Mathf.Sqrt(variance);
        }

        public Quaternion AvgRot(List<Quaternion> rots) {
            int sampleAm = 0;
            Quaternion avg = new Quaternion();

            foreach (Quaternion rot in rots) {
                sampleAm++;
                avg = Quaternion.Slerp(avg, rot, 1f / sampleAm);
            }

            return avg;
        }

        public Vector3 AvgPos(List<Vector3> poss) {
            Vector3 avg = new Vector3();

            foreach (Vector3 pos in poss) {
                avg += pos / poss.Count;
            }

            return avg;
        }
    }

}