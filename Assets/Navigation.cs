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
        public List<MessageTypes.Geometry.Twist> messageList = new List<MessageTypes.Geometry.Twist>();
        public bool isRecording;
        public bool playRecording;
        public ButtonBehavior buttonManager;
        public bool emergencyStop;

        public float linearMultiplier;
        public float angularMultiplier;
        public bool buttonLeft;
        public bool buttonRight;
        public bool buttonForward;
        public bool buttonBackward;

        private int aboutFaceIterator;

        private int ABOUT_FACE_SEQ_LEN = 230;

        public bool resetList() {
            isRecording = false;
            messageList.Clear();
            return true;
        }

        private MessageTypes.Geometry.Twist message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private MessageTypes.Geometry.Twist InitAboutFaceVector(int direction)
        {
            MessageTypes.Geometry.Vector3 lin = new MessageTypes.Geometry.Vector3(0, 0, 0);
            MessageTypes.Geometry.Vector3 ang = new MessageTypes.Geometry.Vector3(0, 0, direction);
            MessageTypes.Geometry.Twist twist = new MessageTypes.Geometry.Twist();
            twist.linear = lin;
            twist.angular = ang;

            return twist;
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Twist();
            message.linear = new MessageTypes.Geometry.Vector3();
            message.angular = new MessageTypes.Geometry.Vector3();
        }
        private void UpdateMessage()
        {
            float inputForward = Input.GetAxis("Vertical");
            float inputTurn = Input.GetAxis("Horizontal");

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
            
            // Debug.Log("isRecording: " + isRecording); 
            if (isRecording) {
                messageList.Add(new MessageTypes.Geometry.Twist(message.linear, message.angular));
                // Debug.Log("message: x: " + message.angular.x + ", y: " + message.angular.y + ", z: " + message.angular.z);
            } else if (playRecording) {
                Debug.Log("playing recording");
                if(messageList.Count > 0) {
                    // about face
                    Debug.Log("aboutFaceIter: " + aboutFaceIterator);
                    if (aboutFaceIterator < ABOUT_FACE_SEQ_LEN)
                    {
                        message = InitAboutFaceVector(1);
                        Debug.Log("message: x: " + message.angular.x + ", y: " + message.angular.y + ", z: " + message.angular.z);
                        aboutFaceIterator++;
                    } else {
                        // playback record
                        Debug.Log("playing back; messageList.Count: " + messageList.Count);
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
                        Debug.Log("Done playing. aboutFaceIter: " + aboutFaceIterator);
                    }
                }

                if (emergencyStop)
                {
                    playRecording = false;
                    resetList();
                    emergencyStop = false;
                }
            }

            // Debug.Log(message);
            Publish(message);
        }

        public void resetAboutFaceIterator()
        {
            aboutFaceIterator = 0;
        }

        private void negateVector(MessageTypes.Geometry.Vector3 vector) {
            vector.x = -vector.x;
            vector.y = -vector.y;
            vector.z = -vector.z;
        }

        private void negateTwist(MessageTypes.Geometry.Twist twist) {
            // negateVector(twist.linear);
            negateVector(twist.angular);
        }

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
