/*
	Created by Carl Emil Carlsen.
	Copyright 2016 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BigEyes
{
	public class OSCController : MonoBehaviour
	{
		public OscOut OscOut;
        public string IpAddress;
        public int Port;

        public bool ControllerActive = false;

		void Start()
		{
			// Ensure that we have a OscOut component.
			if( !OscOut ) OscOut = gameObject.AddComponent<OscOut>();

            // Prepare for sending messages to applications on this device on port 7000.
            //oscOut.Open( port );

            // Or, to a target IP Address (Unicast).
            //oscOut.Open( 7000, "192.168.1.101" );

            // Or to all devices on the local network (Broadcast).
            // oscOut.Open( port, "255.255.255.255" );

            // Or to a multicast group (Multicast).
            //oscOut.Open( 7000, "224.1.1.101" );

            OscOut.Open(Port, IpAddress);

            // Init some Ableton Live stuff
            // Init loop length of Midi Loopers
            SendOSC("/be/track/1/loop/1/length", 0.2f);
            SendOSC("/be/track/2/loop/1/length", 0.2f);
            SendOSC("/be/track/3/loop/1/length", 0.2f);
            SendOSC("/be/track/4/loop/1/length", 0.2f);
            SendOSC("/be/track/5/loop/1/length", 0.2f);
        }


		void Update()
		{
			//// Send a message with one float argument.
			//oscOut.Send( "/test1", Random.value );

			//// Send a message with a number of assorted argument types.
			//oscOut.Send( "/test2", Random.value, "Text", false );

			//// Create a message and send it.
			//OscMessage message = new OscMessage( "/test3" );
			//message.Add( "Allo" );
			//message.Add( "World" );
			//message.args[0] = "Hello"; // Let's say we want overwrite the first argument
			//oscOut.Send( message );
		}

        public void SendOSC(string oscPath, float value)
        {
            OscOut.Send(oscPath, value);

        }
        public void SendOSC(string oscPath, float value, float value2)
        {
            OscOut.Send(oscPath, value, value2);

        }
        public void SendOSC(string oscPath, int value, int value2)
        {
            OscOut.Send(oscPath, value, value2);

        }
    }
}