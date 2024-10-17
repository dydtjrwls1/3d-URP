using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static void ResetAndRunCoroutine(MonoBehaviour owner, ref Coroutine coroutine, Func<IEnumerator> routine)
    {
        if(owner == null)
        {
            Debug.LogWarning("Coroutine Owner is Null.");
            return;
        }

        if (coroutine != null)
        {
            owner.StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = owner.StartCoroutine(routine());
    }
}
