using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string userName;
    public int currScore;
    [SerializeField]
    private bool Horror;

    [SerializeField]
    private bool RelaxationToHorror;
    [SerializeField]
    private bool AttentionTrigger;
    [SerializeField]
    private bool RelaxationTrigger;
    [SerializeField]
    private bool AttentionTriggerProtector;
    [SerializeField]
    private bool RelaxationTriggerProtector;
    [SerializeField]
    private float AttentionTimer;
    [SerializeField]
    private float RelaxationTimer;
    [SerializeField]
    private float HoldingTime_Attention;
    [SerializeField]
    private float HoldingTime_Relaxation;
    [SerializeField]
    private float DelayTime_Attention;
    [SerializeField]
    private float DelayTime_Relaxation;

    [SerializeField]
    private bool isDefense;

    void Start()
    {
        currScore = 0;
        isDefense = false;
    }
    
    void FixedUpdate() {

        if(Horror) {
            RelaxationTimer += Time.fixedDeltaTime;
            if(RelaxationTimer > Time.fixedDeltaTime) {
                HorrorSet(false);
                RelaxationTimer = 0.0f;
            }
        }
        /*
        if (AttentionTriggerProtector) {
            AttentionTimer += Time.deltaTime;
            if (AttentionTimer > HoldingTime_Attention)
                AttentionTrigger = false;
            if (AttentionTimer > HoldingTime_Attention + DelayTime_Attention)
                AttentionTriggerProtector = false;
        }
        if (RelaxationTriggerProtector)
        {
            RelaxationTrigger = false;
            RelaxationTimer += Time.deltaTime;
            if (RelaxationTimer > HoldingTime_Relaxation)
                RelaxationToHorror = false;
            if (RelaxationTimer > HoldingTime_Relaxation + DelayTime_Relaxation)
                RelaxationTriggerProtector = false;
        }
        if (RelaxationToHorror)
        {
            Horror = RelaxationTrigger;
        }
        else
        {
            Horror = AttentionTrigger;
        }*/

    }


    public bool HorrorGetter(){
        return Horror;
    }
    public void HorrorSet(bool temp){
        Horror=temp;
    }
    public void AttentionTriggerSetter() {
        if (!AttentionTriggerProtector)
        {
            AttentionTrigger = true;
            AttentionTriggerProtector = true;
            AttentionTimer = 0;
        }
    }

    public bool isDefenseGetter(){
        return isDefense;
    }
    public void isDefenseSetter(bool temp){
        isDefense = temp;
    }

    public void RelaxationTriggerSetter()
    {
        if(!isDefense) {
            HorrorSet(true);
        }
        /*
        if (!RelaxationTriggerProtector)
        {
            RelaxationTrigger = true;
            RelaxationTriggerProtector = true;
            RelaxationTimer = 0;
            RelaxationToHorror= true;
        }*/

    }
}
