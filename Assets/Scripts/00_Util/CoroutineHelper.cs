using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public void HelpDODisable(GameObject gameObject, float duration)
    {
        StartCoroutine(AutoDisableCoroutine(gameObject, duration));
    }

    IEnumerator AutoDisableCoroutine(GameObject gameObject, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            yield return null;
        }
        gameObject.SetActive(false);
        Destroy(this.gameObject); 
    }
}
