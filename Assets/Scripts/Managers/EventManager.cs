using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject[] basicOrbSpawner;
    [SerializeField] private GameObject[] stoneSpawner;
    [SerializeField] private GameObject[] SpecialOrbSpawner;

    public bool GameClear = false;
    
    [SerializeField] private float EventStartDelayTime;
    [SerializeField] private float BasicSpawnTime;
    [SerializeField] private float swordTime;

    [SerializeField] private GameObject explainUI; 

    // tutorial UI �??��
    [SerializeField] private GameObject magicObj;  
    [SerializeField] private GameObject specialObj;  
    [SerializeField] private GameObject stoneObj; 

    // Animator �??�� 
    Animator animator1A;
    Animator animator1B;
    Animator animator1C;
    Animator animator2A;
    Animator animator2B;
    Animator animator3A;
    Animator animator3B;

    private float Timer = 0;

    public Coroutine EventFlow = null;

    void Start(){
        // ?��?��메이?�� �??�� ?��?��?��?��?��,, ?���? 조금 ?�� ?��?��?��?���? ?��리하?�� 방법?�� 찾아보겠?��?�� ?��
        animator1A = magicObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // magicObj?�� step1 ?��?��메이?�� �??��?���?
        animator1B = magicObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // magicObj?�� step2 ?��?��메이?�� �??��?���?
        animator1C = magicObj.transform.GetChild(2).gameObject.GetComponent<Animator>();  // magicObj?�� step3 ?��?��메이?�� �??��?���?
        animator2A = specialObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // speicalObj?�� step1 ?��?��메이?�� �??��?���?
        animator2B = specialObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // speicalObj?�� step2 ?��?��메이?�� �??��?���?
        animator3A = stoneObj.transform.GetChild(0).gameObject.GetComponent<Animator>();  // stoneObj?�� step1 ?��?��메이?�� �??��?���?
        animator3B = stoneObj.transform.GetChild(1).gameObject.GetComponent<Animator>();  // stoneObj?�� step2 ?��?��메이?�� �??��?���?

        animator1A.SetBool("isDone", false);
        animator1B.SetBool("isDone", false);
        animator1C.SetBool("isDone", false);
        animator2A.SetBool("isDone", false);
        animator2B.SetBool("isDone", false);
        animator3A.SetBool("isDone", false);
        animator3B.SetBool("isDone", false);
    }

    void Awake()  
    {
        BasicSpawnStop(true);  
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true);
    }


    public IEnumerator EventFlowCoroutine()
    {   
       
        //start window 

        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        
        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop(false); 
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 

        StoneSpawnStop(false); 
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().beat *=3;
        }
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
        yield return new WaitForSeconds(5);
        
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public IEnumerator TutorialEventFlow()
    {   
        //[**** 게임 ?��계�?? explain window ****]
        explainUI.SetActive(true);
        yield return new WaitForSeconds(10);
        explainUI.SetActive(false); 

        //[****마법 ?���? ?���? 방법 window***]
        //step1 UI
        magicObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 �Ϸ����� �� ����){
        //     animator1A.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        magicObj.transform.GetChild(1).gameObject.SetActive(true);    // step2 UI
        // if(step2 �??�� ?��료하�?){
        //     animator1B.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(1).gameObject);
        // }

        //step3 UI
        magicObj.transform.GetChild(2).gameObject.SetActive(true);    // step2 UI
        // if(step3 �??�� ?��료하�?){
        //     animator1C.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(magicObj.transform.GetChild(2).gameObject);
        // }

        yield return new WaitForSeconds(20);
 
        BasicSpawnStop(false);
        yield return new WaitForSeconds(BasicSpawnTime);

        //"Don't take your eyes off the starry Orb! Put all my energy into your eyes!" It's time to say that
        yield return new WaitForSeconds(7);
        

        //[****?���? ?���? ?���? 방법 window***]
        //step1 UI
        specialObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 �??�� ?��료하�?){
        //     animator2A.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(specialObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        specialObj.transform.GetChild(1).gameObject.SetActive(true);    // step1 UI
        // if(step2 �??�� ?��료하�?){
        //     animator2B.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(specialObj.transform.GetChild(1).gameObject);
        // }
        yield return new WaitForSeconds(20);

        BasicSpawnStop(true);
        SpecialOrbSpawnAllStop(false); 
        while (!SpecialOrbSpawner[0].GetComponent<SpecialOrbSpawner>().isSpawnStop)
        {
            yield return null;
        }

        
        //[****?��?��?�� ?���? 방법 window***]
        //step1 UI
        stoneObj.transform.GetChild(0).gameObject.SetActive(true);    // step1 UI
        // if(step1 �??�� ?��료하�?){
        //     animator3A.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(stoneObj.transform.GetChild(0).gameObject);
        // }

        //step2 UI
        stoneObj.transform.GetChild(01).gameObject.SetActive(true);    // step1 UI
        // if(step2 �??�� ?��료하�?){
        //     animator3B.SetBool("isDone", true);  // ?��?���?
        //     yield return new WaitForSeconds(2);
        //     Destroy(stoneObj.transform.GetChild(1).gameObject);
        // }

        yield return new WaitForSeconds(20);

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(swordTime); 
        
        //It's time to let you know that the power of your eyes is back
        StoneSpawnStop(true); 
        yield return new WaitForSeconds(7); 
        
        BasicSpawnStop(false); 
        yield return new WaitForSeconds(10); 

        StoneSpawnStop(false); 
        yield return new WaitForSeconds(BasicSpawnTime-10); 
        

        BasicSpawnStop(true); 
        StoneSpawnStop(true); 
        SpecialOrbSpawnAllStop(true); 
        yield return new WaitForSeconds(5);
        
        GameClear = true;
        // Reset
        //eventStarted = false;
        //EventFlow = null;
    }

    public void BasicSpawnStop(bool stop)
    {
        foreach (GameObject spawner in basicOrbSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    public void StoneSpawnStop(bool stop)
    {
        foreach (GameObject spawner in stoneSpawner)
        {
            spawner.GetComponent<Spawner>().isSpawnStop = stop;
        }
    }

    public void SpecialOrbSpawnAllStop(bool stop)
    {
        foreach (GameObject spawner in SpecialOrbSpawner)
        {
            spawner.GetComponent<SpecialOrbSpawner>().isSpawnStop = stop;
        }
    }
}
