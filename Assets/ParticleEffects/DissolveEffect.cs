using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour {

    public float spawnEffectTime = 2;
    //public float pause = 1;
    public AnimationCurve fadeIn;

    ParticleSystem ps_extraEffects;
    float timer = 0;
    Renderer _renderer;

    int shaderProperty;
    private bool canShow = false;

    void Start() {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps_extraEffects = GetComponentInChildren<ParticleSystem>();
        //gets right particle effects but doesnt play it in update
        //Debug.Log(ps_extraEffects.name);

        var main = ps_extraEffects.main;
        main.duration = spawnEffectTime;

        //ps.Play();

    }
    /* original for show
	void Update ()
    {
        if (timer < spawnEffectTime + pause)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ps.Play();
            timer = 0;
        }


        _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate( Mathf.InverseLerp(0, spawnEffectTime, timer)));
        
    }*/
    void Update() {
        
        if (Input.GetKeyDown("k")) {
            canShow = true;
        }
        if (canShow) {
            if (timer < spawnEffectTime) {
                timer += Time.deltaTime;
            }
            else {
                //timer = 0;
            }
            ps_extraEffects.Play();
            _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
        }
    }

}

