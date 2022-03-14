/*
* Filename: Navigation.cs
* Student: Arthi Haripriyan, Pratyusha Ghosh, Alex Chow, Saikiran Komatieni
* Final Project: Bot-The-Builder
*
* Description: This file defines core rosconnector interface to control the robot.
* Buttons are setup as member variables. The autonomous navigation code is also   implemented here. The class handles multiple modalities (WASD, arrow keys 
* and screen button presses) for controlling the robot. 
*
* How to use:
* Usage:
* This file must be run as part of the unity package
*/

/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class Navigation : UnityPublisher<MessageTypes.Geometry.Twist>
    {

        //Stores the list of moves that were published to the robot
        public List<MessageTypes.Geometry.Twist> messageList = new List<MessageTypes.Geometry.Twist>();

        //member variables to store information about the status of each button 
        public bool isRecording;
        public bool playRecording;
        public ButtonBehavior buttonManager;
        public bool emergencyStop;
        public bool buttonLeft;
        public bool buttonRight;
        public bool buttonForward;
        public bool buttonBackward;

        //scaling factors for motor control publish messages
        public float linearMultiplier;
        public float angularMultiplier;

        private int aboutFaceIterator;

        private int ABOUT_FACE_SEQ_LEN = 230;
        private MessageTypes.Geometry.Twist message;

        /*
        Name: Start()
        Purpose: Initial method once RosbridgeClient is set up
        */
        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        /*
        Name: resetList()
        Purpose: Reset the list of control messages that were sent to the robot
        */
        public bool resetList() {
            isRecording = false;
            messageList.Clear();
            return true;
        }

        /*
        Name: InitAboutFaceVector(int direction)
        Purpose: Initialie the linear and angular commands to be sent to the robot 
        @input direction: sets the direction that the robot initally faces
        */
        private MessageTypes.Geometry.Twist InitAboutFaceVector(int direction)
        {
            MessageTypes.Geometry.Vector3 lin = new MessageTypes.Geometry.Vector3(0, 0, 0);
            MessageTypes.Geometry.Vector3 ang = new MessageTypes.Geometry.Vector3(0, 0, direction);
            MessageTypes.Geometry.Twist twist = new MessageTypes.Geometry.Twist();
            twist.linear = lin;
            twist.angular = ang;

            return twist;
        }

        /*
        Name: FixedUpdate()
        Purpose: method automatically gets called at a pre-defined frequency set by RosBridgeClient
        */
        private void FixedUpdate()
        {
            UpdateMessage();
        }

        /*
        Name: InitializeMessage()
        Purpose: Initialize the Twist message to send to the robot
        */
        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Twist();
            message.linear = new MessageTypes.Geometry.Vector3();
            message.angular = new MessageTypes.Geometry.Vector3();
        }

        /*
        Name: UpdateMessage()
        Purpose: Logic for what happens at each update step. First, the state of all butttons are read
        and based on that the appropriate Twist message is published to the robot.
        */        
        private void UpdateMessage()
        {
            //Reading keystrokes
            float inputForward = Input.GetAxis("Vertical");
            float inputTurn = Input.GetAxis("Horizontal");

            //Reading button states
            if(buttonForward) {
                inputForward = 1;
            }
            if(buttonBackward) {
                inputForward = -1;
            }
            if (buttonLeft)
            {
                inputTurn = -1;
            }
            if (buttonRight)
            {
                inputTurn = 1;
            }

            float forward = inputForward * linearMultiplier;
            float turn = inputTurn * angularMultiplier;

            Vector3 linearVelocity = new Vector3(0, 0, forward);
            Vector3 angularVelocity = new Vector3(0, -turn, 0);
            message.linear = GetGeometryVector3(linearVelocity.Unity2Ros());
            message.angular = GetGeometryVector3(angularVelocity.Unity2Ros());
            
            //store control command in list if we are recording 
            if (isRecording) {
                messageList.Add(new MessageTypes.Geometry.Twist(message.linear, message.angular));
            } else if (playRecording) {
                if(messageList.Count > 0) {
                    // about face
                    if (aboutFaceIterator < ABOUT_FACE_SEQ_LEN)
                    {
                        message = InitAboutFaceVector(1);
                        aboutFaceIterator++;
                    } else {
                        // playback record
                        int last = messageList.Count - 1;
                        message = messageList[last];
                        negateTwist(message);
                        messageList.RemoveAt(last);
                    }
                } else {
                    // done playing
                    if (aboutFaceIterator < ABOUT_FACE_SEQ_LEN * 2)
                    {
                        message = InitAboutFaceVector(-1);
                        aboutFaceIterator++;
                    } else
                    {
                        playRecording = false;
                        buttonManager.onFinishingReturn();
                    }
                }

                //reset controls if emergency button is pressed
                if (emergencyStop)
                {
                    playRecording = false;
                    resetList();
                    emergencyStop = false;
                }
            }

            Publish(message);
        }

        /*
        Name: resetAboutFaceIterator()
        Purpose: reset utility
        */     
        public void resetAboutFaceIterator()
        {
            aboutFaceIterator = 0;
        }

        /*
        Name: negateVector(MessageTypes.Geometry.Vector3 vector)
        Purpose: multiply all values in vector by -1
        @input vector: Vector3 Ros message
        */
        private void negateVector(MessageTypes.Geometry.Vector3 vector) {
            vector.x = -vector.x;
            vector.y = -vector.y;
            vector.z = -vector.z;
        }

        /*
        Name: negateTwist(MessageTypes.Geometry.Twist twist)
        Purpose: negate the twist message
        @input twist: Twist Ros message
        */
        private void negateTwist(MessageTypes.Geometry.Twist twist) {
            negateVector(twist.angular);
        }

        /*
        Name: GetGeometryVector3(Vector3 vector3)
        Purpose: Convert a standard Vector3 to a Ros Vector3 type
        @input vector3: Standard Vector 3 type
        */
        private static MessageTypes.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            MessageTypes.Geometry.Vector3 geometryVector3 = new MessageTypes.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }
    }
}
