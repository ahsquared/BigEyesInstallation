﻿/*
	Created by Carl Emil Carlsen.
	Copyright 2016 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OscSimpl.Examples
{
	public class UsingTimeTags : MonoBehaviour
	{
		public GameObject uiWrapper;
		public Text sendLabel;
		public Text receiveLabel;
		public bool sendAsBundle;

		OscOut oscOut;
		OscIn oscIn;

		const string address = "/test";


		void Start()
		{
			// Create objects for sending and receiving.
			oscOut = gameObject.AddComponent<OscOut>();
			oscIn = gameObject.AddComponent<OscIn>(); 

			// Prepare for sending messages on port 7000.
			oscOut.Open( 7000 );

			// Prepare for receiving messages on port 7000.
			oscIn.Open( 7000 );

			// Forward recived messages with address to method.
			oscIn.Map( address, OnMessageReceived );

			// Show UI.
			uiWrapper.SetActive( true );
		}


		void Update()
		{
			// If we are going to send 'as bundle', then we would like OSC io
			// to add the timetag to the messeges contained in the bundle.
			oscIn.addTimeTagsToBundledMessages = sendAsBundle;

			// Create a messege
			OscMessage message = new OscMessage( address );

			// Create a timetag. Default time is DateTime.Now.
			OscTimeTag timetag = new OscTimeTag();

			// Make it 1 milisecond into the future.
			timetag.time = timetag.time.AddMilliseconds( 1 );

			// Two possible methods for sending timetags ...
			if( sendAsBundle )
			{
				// Either create a bundle with the timetag, add the message and send.
				OscBundle bundle = new OscBundle( timetag );
				bundle.Add( message );
				oscOut.Send( bundle );
			} else {
				// Or add the timetag to message and send it.
				message.Add( timetag );
				oscOut.Send( message );
			}

			// Update label.
			sendLabel.text = timetag.time + ":" + timetag.time.Millisecond;
		}


		void OnMessageReceived( OscMessage message )
		{
			// Get the time tag.
			OscTimeTag timeTag;
			if( !message.TryGet( 0, out timeTag ) ) return;

			// Update label.
			receiveLabel.text = timeTag.time + ":" + timeTag.time.Millisecond;
		}
	}
}