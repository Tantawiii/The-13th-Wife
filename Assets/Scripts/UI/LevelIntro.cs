using System.Collections;
using UnityEngine;

public class LevelIntro : MonoBehaviour
{
    [SerializeField] private UI_Curtain curtain;
    [SerializeField] private UI_TypewriterText typewriter;
    [SerializeField] private float startDelay = 1f;
    [TextArea]
    [SerializeField] private string introLine = "12 ex wives, survive the way you want either win the argument against them or run from them till they succumb to the pit";

    private void Start()
    {
        curtain?.Open();
        StartCoroutine(PlayIntroCo());
    }

    private IEnumerator PlayIntroCo()
    {
        yield return new WaitForSeconds(startDelay);

        typewriter?.Play(introLine);
    }
}
