using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UI_Curtain : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName = "curtains_Close_VFX";
    [SerializeField] private float clipLength = 0.9f;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(OpenCo());
    }

    private IEnumerator OpenCo()
    {
        yield return Scrub(1f, 0f);
        gameObject.SetActive(false);
    }

    public IEnumerator PlayClose()
    {
        gameObject.SetActive(true);
        yield return Scrub(0f, 1f);
    }

    private IEnumerator Scrub(float from, float to)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.speed = 0f;

        float elapsed = 0f;
        while (elapsed < clipLength)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / clipLength));
            animator.Play(stateName, 0, normalizedTime);
            animator.Update(0f);
            yield return null;
        }

        animator.Play(stateName, 0, to);
        animator.Update(0f);
    }
}
