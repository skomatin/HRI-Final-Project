/*
* Filename: ButtonBehavior.cs
* Student: Arthi Haripriyan, Pratyusha Ghosh, Alex Chow, Saikiran Komatieni
* Final Project: Bot-The-Builder
*
* Description: This file implements the logic and callbacks for all interactive
* buttons in the interface. The main class establishes a connection to all 
* Ros Connectors by declaring them as a member variables. The class also
* uses member variabels to keep track of the state of all buttons and
* interface.
*
* How to use:
* Usage:
* This file must be run as part of the unity package
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using UnityEngine.UI;


/*
Main class that handles functionality of buttons
*/
public class ButtonBehavior : MonoBehaviour {
    private string PROMPT_TO_START_RECORDING = "Press the Start Recording button once you decide to move!";
    private string PROMPT_ONCE_OBJECT_FOUND  = "Press the Object Found button once you have found the desired object!";
    private string PROMPT_TO_RETURN_TO_CITY  = "Press the Return to Main City button once the object has been placed in the basket!";
    private string PROMPT_RETURNING = "Returning...";

    //Rosconnector connections for navigation and communication
    public RosSharp.RosBridgeClient.Chatter rosConnector2;
    public RosSharp.RosBridgeClient.ChatterListen rosConnectorListen;
    public RosSharp.RosBridgeClient.Navigation rosConnector;

    //Member variables to connect to buttons
    public Button btnStartRecording;
    public Button btnObjectFound;
    public Button btnReturnToCity;
    public Button btnEmergencyStop;
    public Text prompt;

    //Member variables to keep track of button states
    private bool isRecording;
    private bool buttonPressedRight;
    private bool buttonPressedLeft;
    private bool buttonPressedForward;
    private bool buttonPressedBackward;

    /*
    Name: OnButtonPressRight()
    Purpose: Callback for the right button press
    */
    public void OnButtonPressRight(){
        buttonPressedRight = true;                                      //Update local State
        rosConnector.buttonRight = true;                                //Update Rosconnector button state
    }

    /*
    Name: OnButtonPressLeft()
    Purpose: Callback for the left button press
    */
    public void OnButtonPressLeft()
    {
        buttonPressedLeft = true;
        rosConnector.buttonLeft = true;
    }

    /*
    Name: OnButtonPressForward()
    Purpose: Callback for the forward button press
    */
    public void OnButtonPressForward()
    {
        buttonPressedForward = true;
        rosConnector.buttonForward = true;
    }

    /*
    Name: OnButtonPressBackward()
    Purpose: Callback for the acward button press
    */
    public void OnButtonPressBackward()
    {
        buttonPressedBackward = true;
        rosConnector.buttonBackward = true;
    }

    /*
    Name: OnButtonUpRight()
    Purpose: Callback for when right and other buttons are pressed
    */
    public void OnButtonUpRight()
    {
        buttonPressedRight = false;
        rosConnector.buttonRight = false;
    }

    /*
    Name: OnButtonUpLeft()
    Purpose: Callback for when left and other buttons are pressed
    */
    public void OnButtonUpLeft()
    {
        buttonPressedLeft = false;
        rosConnector.buttonLeft = false;
    }

    /*
    Name: OnButtonUpForward()
    Purpose: Callback for when forward and other buttons are pressed
    */
    public void OnButtonUpForward()
    {
        buttonPressedForward = false;
        rosConnector.buttonForward = false;
    }

    /*
    Name: OnButtonUpBackward()
    Purpose: Callback for when backward and other buttons are pressed
    */
    public void OnButtonUpBackward()
    {
        buttonPressedBackward = false;
        rosConnector.buttonBackward = false;
    }

    /*
    Name: OnButtonRecordPress()
    Purpose: Callback for start recording button
    */
    public void OnButtonRecordPress()
    {
        btnStartRecording.interactable = false;                             //Disable start recording
        btnObjectFound.interactable = true;                                 //Enable object found button
        btnReturnToCity.interactable = false;                               //Keep return to city disable (till helper pressesdone)
        btnEmergencyStop.interactable = false;                              //only enable emergency button during autonomous navigation

        isRecording = true;
        rosConnector.isRecording = true;

        prompt.text = PROMPT_ONCE_OBJECT_FOUND;

        rosConnector2.sendMessage(rosConnector2.NAVIGATING);                //Publish message to turtlebot
    }

    /*
    Name: OnButtonObjectFoundPress()
    Purpose: Callback for once oobject is found
    */
    public void OnButtonObjectFoundPress()
    {
        btnStartRecording.interactable = false;
        btnObjectFound.interactable = false;
        btnEmergencyStop.interactable = false;

        isRecording = false;
        rosConnector.isRecording = false;
        rosConnector.resetAboutFaceIterator();

        prompt.text = PROMPT_TO_RETURN_TO_CITY;

        rosConnector2.sendMessage(rosConnector2.REQUEST_OBJECT);
    }

    /*
    Name: OnButtonReturnToCityPress()
    Purpose: Callback for return button
    */
    public void OnButtonReturnToCityPress()
    {
        prompt.text = PROMPT_RETURNING;

        btnStartRecording.interactable = false;
        btnObjectFound.interactable = false;
        btnReturnToCity.interactable = false;
        btnEmergencyStop.interactable = true;

        if (!isRecording) {
            rosConnector.playRecording = true;                                  //Only enable recording if it has not alread been turned on
        }
    }

    /*
    Name: OnButtonEmergencyStopPress()
    Purpose: Callback for emergency stop button
    */
    public void OnButtonEmergencyStopPress()
    {
        rosConnector.emergencyStop = true;
        onFinishingReturn();
        rosConnector2.sendMessage(rosConnector2.TASK_COMPLETE);                 //Convy status information to the turtlebot
        rosConnectorListen.done = false;
    }

    /*
    Name: onFinishingReturn()
    Purpose: Method that gets called once autonomous navigation is complete
    */
    public void onFinishingReturn()
    {
        btnStartRecording.interactable = true;
        btnObjectFound.interactable = false;
        btnReturnToCity.interactable = false;
        btnEmergencyStop.interactable = false;

        prompt.text = PROMPT_TO_START_RECORDING;

        rosConnector2.sendMessage(rosConnector2.TASK_COMPLETE);
        rosConnectorListen.done = false;                                        //Once task is complete stop listening to topic
    }
}