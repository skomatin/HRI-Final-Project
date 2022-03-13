using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class Chatter : UnityPublisher<MessageTypes.Std.String>
    {
        public bool testButtonBool = false;
        public string NAVIGATING = "navigating";
        public string REQUEST_OBJECT = "request_object";
        // public string DONE = "done";
        public string TASK_COMPLETE = "task_complete";
        public string RESET = "reset";

        protected override async void Start()
        {
            base.Start();
            // WaitForSeconds(3);
            // sendMessage(RESET);
            for(int i = 0; i < 10; i++) {
                sendMessage(RESET);
            }

        }

        public void sendMessage(string message) {
            MessageTypes.Std.String sendString = new MessageTypes.Std.String(message);
            Debug.Log("message sending: "+message);
            Publish(sendString);
            Debug.Log("message sent: "+message);
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void UpdateMessage()
        {
        }

        
    }
}