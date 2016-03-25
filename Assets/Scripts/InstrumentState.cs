using System;
using UnityEngine;
using System.Collections;
using ProgressBar;

namespace BigEyes
{



    public class InstrumentState : MonoBehaviour
    {

        public float TrackNumber = 1;
        public bool RecordEnabled = false;
        public OscIn OscIn;
        public int Bar;
        public int Beat;
        public float Bpm;

        public ProgressBarBehaviour BarBeatProgress;

        // Use this for initialization
        void Start()
        {
            // Ensure that we have a OscIn component.
            if (!OscIn) OscIn = gameObject.AddComponent<OscIn>();

            // Start receiving from unicast and broadcast sources on port 9000.
            OscIn.Open(9000);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {

            // 3) For messages with multiple arguments, provide the address and a method
            // that takes a OscMessage object argument, then process the message manually.
            // See the OnTranportTick method.
            OscIn.Map("/m4l/transport", OnTranportTick);
        }

        void OnDisable()
        {
            // For mapped methods, simply pass them to Unmap.
            OscIn.Unmap(OnTranportTick);

        }

        void OnTranportTick(OscMessage message)
        {
            // Get string arguments at index 0 and 1 safely.

            // BarBeat = "001:001:000"
            string barBeat, bpm;
            if (message.TryGet(0, out barBeat))
            {
                //Debug.Log("Bar/Beat: " + barBeat);
                string[] barBeatArr = barBeat.Split(':');
                Bar = int.Parse(barBeatArr[0]);
                Beat = int.Parse(barBeatArr[1]);
                //Debug.Log("Bar : Beat= " + Bar + " : " + Beat);

                BarBeatProgress.BarBeat = new float[] {Bar, Beat};
                //Debug.Log("progress value: " + (100*(((Bar - 1)*4 + Beat - 1)/16f)));
            }
            if (message.TryGet(1, out bpm))
            {
                Bpm = float.Parse(bpm.Replace("bpm", ""));
            }


            // If you wish to mess with the arguments yourself, you can.
            //foreach (object a in message.args) if (a is string) Debug.Log("Received: " + a);

        }
    }
}

