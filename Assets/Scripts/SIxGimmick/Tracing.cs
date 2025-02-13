using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Tracing : MonoBehaviour 
{
    public enum MovementPattern 
    {
        O,
        Z,
        U
    }
    [SerializeField] private MovementPattern selectedPattern = MovementPattern.O; 

    public GameObject[] targets;
    private Sequence mySequence;
    public float movingTime = 3;
    private Coroutine drawCoroutine;

    [SerializeField] private float HMT = 1;
    [SerializeField] private AudioSource UngSound;
    [SerializeField] private GameObject DestroyVisualEffectObject;
    [SerializeField] private GameObject DestroySoundEffectObject;

    private float hoverDuration = 0f;
    private float maxHoverDuration = 8f; // 10 seconds
    public bool IsHovered { get; set; }
    private bool hasAudioPlayed = false;

    public Vector3 HoverPosition { get; set; }

    public ControllerManager controllerManager;

    private void Awake()
    {
        IsHovered = false;

        controllerManager = GameObject.Find("OVRInPlayMode").GetComponent<ControllerManager>();
    }

    private void Update()
    {
        if (drawCoroutine == null)
        {
            StartMovementCoroutine();
        }

        // Check if the object is being gazed upon
        if (IsHovered)
        {
            // Increment the hover duration
            hoverDuration += Time.deltaTime;

            if (!hasAudioPlayed && UngSound != null)
            {
                UngSound.Play();
                hasAudioPlayed = true; // Set the flag to true to indicate audio has played.
            }

            if (hoverDuration >= maxHoverDuration)
            {
                if (transform.parent != null)
                {
                    // Instantiate DestroyEffect at the position of the object
                    Quaternion rotation = Quaternion.Euler(0, 90, 0);
                    GameObject DestroyVisualInstance = Instantiate(DestroyVisualEffectObject, HoverPosition, rotation);
                    // Destroy the instantiated object after 2 seconds
                    Destroy(DestroyVisualInstance, 1f);

                    // Instantiate DestroyEffect at the position of the object
                    GameObject DestroySoundInstance = Instantiate(DestroySoundEffectObject, HoverPosition, Quaternion.identity);
                    // Destroy the instantiated object after 2 seconds
                    Destroy(DestroySoundInstance, 1f);

                    Destroy(transform.parent.gameObject);

                    controllerManager.skillEnergyPoint += controllerManager.attackPoint*8; 
                }
            }
        }
        else
        {
            if (UngSound != null)
            {
                UngSound.Stop();
                hasAudioPlayed = false; // Reset the flag when the object is no longer hovered.
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TargetPosition") && (selectedPattern == MovementPattern.O || selectedPattern == MovementPattern.Z))  
        {
            other.gameObject.transform.position += transform.forward * 4;  
        }
    }

    Sequence Quadrant12(int a)
    {
        return DOTween.Sequence()
            .OnStart(() =>
            {
                transform.DOMoveX(targets[a].transform.position.x, movingTime);
                transform.DOMoveY(targets[a].transform.position.y, movingTime);
                transform.DOMoveZ(targets[a].transform.position.z, movingTime);
            })
            .SetDelay(movingTime);
    }

    Sequence Quadrant34(int a)
    {
        return DOTween.Sequence()
            .OnStart(() =>
            {
                transform.DOMoveX(targets[a].transform.position.x, movingTime * HMT);
                transform.DOMoveY(targets[a].transform.position.y, movingTime * HMT);
                transform.DOMoveZ(targets[a].transform.position.z, movingTime * HMT);
            })
            .SetDelay(movingTime);
    }

    Sequence CircleQuadrant12(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, movingTime).SetEase(Ease.OutQuad);
            transform.DOMoveY(targets[a].transform.position.y, movingTime).SetEase(Ease.InQuad);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime).SetEase(Ease.InQuad);
        })
        .SetDelay(movingTime);
    } 

    Sequence CircleQuadrant34(int a){
        return DOTween.Sequence()
        .OnStart(() => {
            transform.DOMoveX(targets[a].transform.position.x, movingTime).SetEase(Ease.InQuad);
            transform.DOMoveY(targets[a].transform.position.y, movingTime).SetEase(Ease.OutQuad);
            transform.DOMoveZ(targets[a].transform.position.z, movingTime).SetEase(Ease.OutQuad);
        })
        .SetDelay(movingTime);
    } 

    IEnumerator DrawPattern(MovementPattern pattern)
    {
        switch (pattern)
        {
            case MovementPattern.O:
                mySequence = CircleQuadrant12(0)
                    .Append(CircleQuadrant34(1))
                    .Append(CircleQuadrant12(2))
                    .Append(CircleQuadrant34(3));
                break;

            case MovementPattern.Z:
                mySequence = Quadrant12(0)
                    .Append(Quadrant34(1))
                    .Append(Quadrant12(2))
                    .Append(Quadrant34(3));
                break;

            case MovementPattern.U:
                mySequence = CircleQuadrant12(0)
                    .Append(CircleQuadrant34(1))
                    .Append(CircleQuadrant12(2))
                    .Append(CircleQuadrant34(3));
                break;
        }

        yield return new WaitForSeconds(movingTime * 4);
        drawCoroutine = null;
    }

    void StartMovementCoroutine()
    {
        drawCoroutine = StartCoroutine(DrawPattern(selectedPattern));
    }
}
