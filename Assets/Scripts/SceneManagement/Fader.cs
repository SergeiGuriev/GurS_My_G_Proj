using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        
        CanvasGroup canvasGroup;
        Coroutine activeFade = null;
        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();            
        }        

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOutIn()
        {
            while (canvasGroup.alpha < 1)
            {
                yield return FadeOut(2);
                yield return FadeIn(2);
            }
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1,time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (activeFade != null)
            {
                StopCoroutine(activeFade);
            }
            activeFade = StartCoroutine(FadeCoroutine(target, time));
            return activeFade;
        }

        private IEnumerator FadeCoroutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

    }
}
