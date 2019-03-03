using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPoolController : MonoBehaviour
{
    private HackBotPool _bpHackPool;
    private BombBotPool _bpBombPool;
    private TrampBotPool _bpTrampPool;
    private GameObject player;

    void Awake(){
        _bpHackPool = transform.GetChild(0).GetComponent<HackBotPool>();
        _bpBombPool = transform.GetChild(1).GetComponent<BombBotPool>();
        _bpTrampPool = transform.GetChild(2).GetComponent<TrampBotPool>();
        player = GameObject.Find("3rdP_Player");
    }

    public GenericBot GetBombBot(){
        return _bpBombPool.GetObject();
    }

    public GenericBot GetHackBot(){
        return _bpHackPool.GetObject();
    }
    
    public GenericBot GetTrampBot(){
        return _bpTrampPool.GetObject();
    }
}
