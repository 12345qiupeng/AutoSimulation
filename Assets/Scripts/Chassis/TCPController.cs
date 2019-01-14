﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Chassis;
using UnityEngine;
using UnityEngine.UI;

public class TCPController : MonoBehaviour
{

	private ChassisController _cCtrl;
	private SwitchController _navSwitchCtrl;
	private SwitchController _redSwitchCtrl;

	public Text _keyName;
	public Text _startPointName;
	public Text _endPointName;
	public GameObject _redSwitch;

    private bool isRecording = false;

	
	private void Start()
	{
		_cCtrl = GetComponentInParent<ChassisController>();
		_redSwitchCtrl = _redSwitch.GetComponent<SwitchController>();
        isRecording = _redSwitchCtrl.isOn;
		
	}

	// Start is called before the first frame update
    public void OnRecordSwitchChanged()
    {
        isRecording = !isRecording;
        if (isRecording)
	    {
		    _cCtrl._remoteHub.Send($"start record");
	    }
	    else
	    {
		    _cCtrl._remoteHub.Send($"stop record");
	    }
    }

    public void OnNavigationButtonClicked()
    {
	    _cCtrl._remoteHub.Send($"load path {_startPointName.text.ToString()} -> {_endPointName.text.ToString()}");
    }

    public void OnSaveKeyButtonClicked()
    {
	    _cCtrl._remoteHub.Send($"save key pose {_keyName.text.ToString()}");
        var (x, y, θ) = _cCtrl.CurrentPose;
        Functions.LoadObject("KeyPose")
        	.transform
        	.SetPose(x, y, 0, θ + Mathf.PI / 2);
    }
}
