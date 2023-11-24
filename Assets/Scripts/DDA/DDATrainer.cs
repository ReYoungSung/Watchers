using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;

public class DDATrainer : Agent
{
    private bool isRewarding = false;

    private SpawnManager spawnManager;
    private EventManager eventManager;
    private DamagedArea damagedArea;
    private ControllerManagerDDA controllerManager;

    private int MissingPoint;

    private int OriginStageHP;
    private int OriginEnemyHP;

    private float OriginBasicOrbSpeed;
    private float OriginBasicOrbSpawnInterval;
    private float OriginSpecialOrbSpeed;
    private float OriginSpecialOrbSpawnInterval;
    private float OriginStoneSpeed;
    private float OriginStoneSpawnInterval;

    private float initialLevelPoint = 1;

    [SerializeField] private bool isHardDif = false;
    [SerializeField] private bool isEasyDif = false;

    private float LevelPoint;

    private void Awake()  
    {
        eventManager = this.transform.GetComponent<EventManager>(); 
        damagedArea = this.transform.GetComponent<DamagedArea>();
        spawnManager = this.GetComponent<SpawnManager>();
        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManagerDDA>();

        MissingPoint = controllerManager.MissingPoint;
    }

    private void Start()
    {
        StartCoroutine(DecreaseOverTime());
        StartCoroutine(CheckMissingPointChange());

        isRewarding = false;

        OriginStageHP = damagedArea.stageHP;
        OriginEnemyHP = eventManager.EnemyHP;

        OriginBasicOrbSpeed = spawnManager.basicOrbSpeed;
        OriginBasicOrbSpawnInterval = spawnManager.basicOrbSpawnInterval;
        OriginSpecialOrbSpeed = spawnManager.SpecialOrbSpeed;
        OriginSpecialOrbSpawnInterval = spawnManager.SpecialOrbSpawnInterval;
        OriginStoneSpeed = spawnManager.stoneSpeed;
        OriginStoneSpawnInterval = spawnManager.stoneSpawnInterval;
    }

    // ������Ʈ�� ȯ�濡�� �����ϴ� ������ ����
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(damagedArea.stageHP);
        sensor.AddObservation(eventManager.EnemyHP);
        sensor.AddObservation(MissingPoint);

        sensor.AddObservation(spawnManager.basicOrbSpeed);
        sensor.AddObservation(spawnManager.basicOrbSpawnInterval);
        sensor.AddObservation(spawnManager.SpecialOrbSpeed);
        sensor.AddObservation(spawnManager.SpecialOrbSpawnInterval);

        sensor.AddObservation(isHardDif);
        sensor.AddObservation(isEasyDif);

        sensor.AddObservation(initialLevelPoint);
    }

    // ������Ʈ�� �ൿ�� ������ �� ȣ��Ǵ� �޼���
    public override void OnActionReceived(ActionBuffers actionBuffers)  
    {
        //�÷��� �ð��� ���� ���� (�ִ��� Ŭ�����ϰ� ����)
        AddReward(Time.deltaTime);

        //���̵� ������ ���� �ǻ� ����
        if(actionBuffers.DiscreteActions[0] == 1)
        {
            LevelPoint = actionBuffers.ContinuousActions[0]*2;

            if (LevelPoint >= 0.5f && LevelPoint <= 1.5f)  
            { 
                // �ӵ� ����   
                spawnManager.basicOrbSpeed = OriginBasicOrbSpeed * LevelPoint;   
                spawnManager.SpecialOrbSpeed = OriginSpecialOrbSpeed * LevelPoint;   
                spawnManager.stoneSpeed = OriginStoneSpeed * LevelPoint;   

                // ���� ���� ����   
                spawnManager.basicOrbSpawnInterval = OriginBasicOrbSpawnInterval * (2f - LevelPoint);    
                spawnManager.SpecialOrbSpawnInterval = OriginSpecialOrbSpawnInterval * (2f - LevelPoint);    
                spawnManager.stoneSpawnInterval = OriginStoneSpawnInterval * (2f - LevelPoint);     
            } 
            else
                AddReward(-1.0f); 
        }

        // ����� ���̵������� ���� �� ó��   
        if (isHardDif == true)
        {
            if (initialLevelPoint > LevelPoint) 
            {
                AddReward(10.0f);
            }
            else 
            {
                AddReward(-10.0f);
            }

            if (actionBuffers.DiscreteActions[0] == 1) 
            {
                AddReward(5.0f);
            }
            else 
            {
                AddReward(-5.0f);
            }
        }
        else if (isEasyDif == true) // ���� ���̵������� ���� �� ó��
        {
            if (initialLevelPoint > LevelPoint) 
            {
                AddReward(-10.0f);
            }
            else 
            {
                AddReward(10.0f);
            }

            if (actionBuffers.DiscreteActions[0] == 1) 
            {
                AddReward(5.0f); 
            }
            else
            {
                AddReward(-5.0f);
            }
        }
        else
        {
            if (actionBuffers.DiscreteActions[0] == 0) 
            {
                AddReward(20.0f); 
            }
        }
        // ���� �ൿ ���
        initialLevelPoint = actionBuffers.ContinuousActions[0];

        // �н� ���� ���� Ȯ��
        EndMLAgent();
    }

    // �н� ���� ���� Ȯ�� �� ó��
    public void EndMLAgent() 
    {
        if (eventManager.GameClear == true)
        {
            ReviewEnding();
            EndEpisode();
        }

        if (damagedArea.stageHP < 0)
        {
            SetReward(-10000.0f);
            EndEpisode();
        }
    }

    // �н� ���� �� ������ �۾� ����     
    private void ReviewEnding()
    {
        if (isRewarding == false)
        {
            if (damagedArea.stageHP <= 200)
            {
                AddReward(-5000.0f);
            }
            else if (damagedArea.stageHP <= 500)
            {
                AddReward(-2000.0f);
            }

            if (damagedArea.stageHP >= 1800)
            {
                AddReward(-5000.0f);
            }
            else if (damagedArea.stageHP >= 1500) 
            {
                AddReward(-2000.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 10)
            {
                AddReward(2000.0f);
            }

            if (MissingPoint > spawnManager.totalNumOfBasicOrb / 2)
            {
                AddReward(-2000.0f);
            }

            if (damagedArea.stageHP > 700 && damagedArea.stageHP < 1500)
            {
                AddReward(10000.0f);
            }

            isRewarding = true;
        }
    }

    // �ð��� ���� �̺�Ʈ ó��
    private IEnumerator DecreaseOverTime()
    {
        yield return new WaitForSeconds(90f);
        eventManager.EnemyHP -= (OriginEnemyHP + 500);
    }

    // ���� �ð� �������� ä�� ���̵� �ľ�
    private IEnumerator CheckMissingPointChange()
    {
        yield return new WaitForSeconds(20f);

        while (true)  
        { 
            // ó���� ������ MissingPoint ���� ����  
            int initialMissingPoint = MissingPoint;
            // ó���� ������ stageHP ���� ����  
            int initialStageHP = damagedArea.stageHP;

            // 5�� ��ٸ�
            yield return new WaitForSeconds(5f);

            // 5�� �Ŀ� ���� MissingPoint�� ó���� ������ ���� �� 
            int change = MissingPoint - initialMissingPoint;
            int change2 = initialStageHP - damagedArea.stageHP;

            if (change2 >= 150)   //�������� ū �����̸� ����� ���� 
            {
                AddReward(-500.0f);
                isHardDif = true;
                isEasyDif = false;
            }
            else if (change2 == 0) //�Ǽ��� ���� �������� ���� �����̸� ���� ���� 
            {
                AddReward(-500.0f);
                isHardDif = false;
                isEasyDif = true;
            }
            else  
            {
                AddReward(500.0f);
                isHardDif = false;
                isEasyDif = false;   
            }  

            if(change > 5)
            {
                AddReward(-500.0f);
            }

            if (change2 > 500)
            {
                AddReward(-change2*10);
            }


            Debug.Log("isHardDif: " + isHardDif + "/ isEasyDif: " + isEasyDif);

            // ���� MissingPoint ���� �ٽ� ����
            initialMissingPoint = MissingPoint;
        }
    }
}
