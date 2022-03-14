/*
* Filename: ChatterListen.cs
* Student: Arthi Haripriyan, Pratyusha Ghosh, Alex Chow, Saikiran Komatieni
* Final Project: Bot-The-Builder
*
* Description: This file defines the main class to recieve messages from
* the robot via the chatter/robot2unity topic. 
*
* How to use:
* Usage:
* This file must be run as part of the unity package
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Rosconnector code is defined in RosSharp.RosBridgeClient
*/
namespace RosSharp.RosBridgeClient 
{
    /*
    Class inherits from the Subscriber class provided by the RosBridge API
    Message type is set as string for simplicity
    */
    public class ChatterListen : UnitySubscriber<MessageTypes.Std.String> 
    {
        //Connections to buttons to be able to update their properties
        public Button btnReturnToCity;
        public bool done = false;
        public ButtonBehavior bb;

        public string DONE = "done";

        /*
        Name: Start()
        Purpose: Initial method once RosbridgeClient is set up
        */
        protected override void Start()
        {
            base.Start();
        }

        /*
        Name: ReceiveMessage(MessageTypes.Std.String message)
        Purpose: callback for the subscriber. Updates Button properties
        by enabling some and disabling some buttons.
        */
        protected override void ReceiveMessage(MessageTypes.Std.String message) {
            if (message.data.Equals(DONE))
            {
                done = true;
                btnReturnToCity.interactable = true;
                btnReturnToCity.interactable = false;
                btnReturnToCity.interactable = true;
            }
            else {
                Debug.Log("Incorrect message recieved");
            }
        }
    }
}
