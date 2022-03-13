using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient 
{
    public class ChatterListen : UnitySubscriber<MessageTypes.Std.String> 
    {
        public Button btnReturnToCity;
        public bool done = false;
        public ButtonBehavior bb;

        protected override void Start()
        {
            base.Start();
            // base.Topic = "/chatter";
        }

        protected override void ReceiveMessage(MessageTypes.Std.String message) {
            Debug.Log("Receive Message " + message.data);
            if (message.data.Equals("done"))
            {
                Debug.Log("Message Received: done");
                done = true;
                btnReturnToCity.interactable = true;
                btnReturnToCity.interactable = false;
                btnReturnToCity.interactable = true;
            }
        }
    }
}
