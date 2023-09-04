using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DDATrainer : Agent
{

    void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
    	
    }

    public override void CollectObservations(VectorSensor sensor)  
    {
        // ���� ����
        sensor.AddObservation(1);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {    
    	// Agent�� Target������ �̵��ϱ� ���� X, Z�������� Force�� ����
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        // rBody.AddForce(controlSignal * forceMultiplier);

        // Agent�� Target������ �Ÿ��� ����
        float distanceToTarget = 0.1f;

        // Target�� �����ϴ� ��� (�Ÿ��� 1.42���� ���� ���) Episode ����
        if (distanceToTarget < 1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // �÷��� ������ ������ Episode ����
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}