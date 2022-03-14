/*
* Filename: Chatter.cs
* Student: Arthi Haripriyan, Pratyusha Ghosh, Alex Chow, Saikiran Komatieni
* Final Project: Bot-The-Builder
*
* Description: This file defines the main class to communicate messages to
* the robot via the chatter/unity2robot topic. 
*
* How to use:
* Usage:
* This file must be run as part of the unity package
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Rosconnector code is defined in RosSharp.RosBridgeClient
*/
namespace RosSharp.RosBridgeClient
{
    /*
    Class inherits from the Publisher class provided by the RosBridge API
    Message type is set as string for simplicity
    */
    public class Chatter : UnityPublisher<MessageTypes.Std.String>
    {
        public bool testButtonBool = false;

        //String commands to be sent to the robot
        public string NAVIGATING = "navigating";
        public string REQUEST_OBJECT = "request_object";
        public string TASK_COMPLETE = "task_complete";
        public string RESET = "reset";

        /*
        Name: Start()
        Purpose: Initial method once RosbridgeClient is set up
        */
        protected override async void Start()
        {
            base.Start();                                       //Base ctor

            //Called multiple times due to potential lost data during transmission
            //This was only observed to be happening during initialization of 
            //RosBridgeClient
            for(int i = 0; i < 10; i++) {
                sendMessage(RESET);
            }

        }

        /*
        Name: sendMessage(string message)
        Purpose: Implements the code to send the provided message to the robot
        */
        public void sendMessage(string message) {
            //Convert string message to Ros type
            MessageTypes.Std.String sendString = new MessageTypes.Std.String(message);
            Publish(sendString);
        }

        
    }
}