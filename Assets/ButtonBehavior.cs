using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour {
    private string PROMPT_TO_START_RECORDING = "Press the Start Recording button once you decide to move!";
    private string PROMPT_ONCE_OBJECT_FOUND  = "Press the Object Found button once you have found the desired object!";
    private string PROMPT_TO_RETURN_TO_CITY  = "Press the Return to Main City button once the object has been placed in the basket!";
    private string PROMPT_RETURNING = "Returning...";

    public RosSharp.RosBridgeClient.Chatter rosConnector2;
    public RosSharp.RosBridgeClient.ChatterListen rosConnectorListen;
    public RosSharp.RosBridgeClient.Navigation rosConnector;
    public Button btnStartRecording;
    public Button btnObjectFound;
    public Button btnReturnToCity;
    public Button btnEmergencyStop;
    public Text prompt;

    private bool isRecording;
    private bool buttonPressedRight;
    private bool buttonPressedLeft;
    private bool buttonPressedForward;
    private bool buttonPressedBackward;
    private int n;

    public void OnButtonPressRight(){
        buttonPressedRight = true;
        rosConnector.buttonRight = true;
    }

    public void OnButtonPressLeft()
    {
        buttonPressedLeft = true;
        rosConnector.buttonLeft = true;
    }

    public void OnButtonPressForward()
    {
        buttonPressedForward = true;
        rosConnector.buttonForward = true;
    }

    public void OnButtonPressBackward()
    {
        buttonPressedBackward = true;
        rosConnector.buttonBackward = true;
    }

    public void OnButtonUpRight()
    {
        buttonPressedRight = false;
        rosConnector.buttonRight = false;
    }

    public void OnButtonUpLeft()
    {
        buttonPressedLeft = false;
        rosConnector.buttonLeft = false;
    }

    public void OnButtonUpForward()
    {
        buttonPressedForward = false;
        rosConnector.buttonForward = false;
    }

    public void OnButtonUpBackward()
    {
        buttonPressedBackward = false;
        rosConnector.buttonBackward = false;
    }

    public void OnButtonRecordPress()
    {
        // rosConnector.resetList();
        Debug.Log("Record Pressed!\n");

        btnStartRecording.interactable = false;
        btnObjectFound.interactable = true;
        btnReturnToCity.interactable = false;
        btnEmergencyStop.interactable = false;

        isRecording = true;
        rosConnector.isRecording = true;

        prompt.text = PROMPT_ONCE_OBJECT_FOUND;

        Debug.Log("isRecording in BB: " + isRecording);

        rosConnector2.sendMessage(rosConnector2.NAVIGATING);
    }

    public void OnButtonObjectFoundPress()
    {
        Debug.Log("Recording Stopped!\n");

        btnStartRecording.interactable = false;
        btnObjectFound.interactable = false;
        btnEmergencyStop.interactable = false;

        isRecording = false;
        rosConnector.isRecording = false;
        rosConnector.resetAboutFaceIterator();

        prompt.text = PROMPT_TO_RETURN_TO_CITY;

        rosConnector2.sendMessage(rosConnector2.REQUEST_OBJECT);
    }

    public void OnButtonReturnToCityPress()
    {
        prompt.text = PROMPT_RETURNING;

        btnStartRecording.interactable = false;
        btnObjectFound.interactable = false;
        btnReturnToCity.interactable = false;
        btnEmergencyStop.interactable = true;

        if (isRecording) {
            //Something is off
            Debug.Log("Something is off");
        }
        else {
            Debug.Log("Record Playing!\n");
            rosConnector.playRecording = true;
        }
    }

    public void OnButtonEmergencyStopPress()
    {
        rosConnector.emergencyStop = true;
        onFinishingReturn();
        rosConnector2.sendMessage(rosConnector2.TASK_COMPLETE);
        rosConnectorListen.done = false;
    }

    public void onFinishingReturn()
    {
        btnStartRecording.interactable = true;
        btnObjectFound.interactable = false;
        btnReturnToCity.interactable = false;
        btnEmergencyStop.interactable = false;

        prompt.text = PROMPT_TO_START_RECORDING;

        rosConnector2.sendMessage(rosConnector2.TASK_COMPLETE);
        rosConnectorListen.done = false;
    }

    public void setReturnToCityInteractable(bool interactable)
    {
        btnEmergencyStop.interactable = false;
        btnReturnToCity.interactable = interactable;
    }
}