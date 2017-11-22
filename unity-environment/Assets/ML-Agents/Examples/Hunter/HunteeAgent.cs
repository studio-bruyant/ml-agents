﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunteeAgent : Agent {

	public GameObject hunter;
	float huntRadius;
    float step;
    bool touched = false;

	public void Start()
	{
		Academy aca = GameObject.Find("HunterAcademy").GetComponent<HunterAcademy>() as Academy;
		huntRadius = aca.resetParameters["ringRadius"];
		gameObject.transform.localScale = new Vector3(1, 1, 1) * aca.resetParameters["sphereRadius"];
		step = aca.resetParameters["hunteeSpeed"];
	}

	public override List<float> CollectState()
	{
		List<float> state = new List<float>();
		state.Add(gameObject.transform.position.x/ (2 * huntRadius));
		state.Add(gameObject.transform.position.z/ (2 * huntRadius));
		state.Add(hunter.transform.position.x/ (2 * huntRadius));
		state.Add(hunter.transform.position.z/ (2 * huntRadius));
        state.Add((gameObject.transform.position.x - hunter.transform.position.x)/ (2 * huntRadius));
        state.Add((gameObject.transform.position.z - hunter.transform.position.z)/ (2 * huntRadius));
		return state;
	}

	public override void AgentStep(float[] action)
	{
		Vector2 newPosition = new Vector2(
			gameObject.transform.position.x + Mathf.Min(Mathf.Max(-1, action[0]),1) * step,
            gameObject.transform.position.z + Mathf.Min(Mathf.Max(-1, action[1]), 1) * step);
        if (newPosition.magnitude > huntRadius)
        {
            //newPosition = newPosition * huntRadius / newPosition.magnitude;
            reward = -1f;
            done = true;
        }
		gameObject.transform.position =
        	  new Vector3(newPosition.x, gameObject.transform.position.y, newPosition.y);
		if (touched)
		{
			done = true;
			reward = -1f;
		}
        if (!done)
		{
			reward = 0.01f;
		}
        Monitor.Log("reward:", reward*10, MonitorType.slider, gameObject.transform);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == hunter)
		{
			touched = true;
		}
	}

	public override void AgentReset()
	{
		HunterAcademy aca = GameObject.Find("HunterAcademy").GetComponent<HunterAcademy>();
		gameObject.transform.position = new Vector3((Random.value - 0.5f) * huntRadius, gameObject.transform.position.y, (Random.value - 0.5f) * huntRadius);

		while ((gameObject.transform.position - hunter.transform.position).magnitude < aca.resetParameters["sphereRadius"] * 3)
		{
            gameObject.transform.position = new Vector3((Random.value- 0.5f) * huntRadius, gameObject.transform.position.y, (Random.value - 0.5f) * huntRadius);
		}
        touched = false;
		//gameObject.transform.position = new Vector3(huntRadius / 2, gameObject.transform.position.y, 0);
    }

	public override void AgentOnDone()
	{

	}
}
