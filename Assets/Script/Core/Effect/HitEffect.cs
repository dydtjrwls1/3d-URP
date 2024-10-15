using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : RecycleObject
{
    ParticleSystem m_hitEffect; 

    private void Awake()
    {
        m_hitEffect = GetComponent<ParticleSystem>();
    }

    protected override void OnReset()
    {
        DisableTimer(m_hitEffect.main.duration);
    }
}
