using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WoodAssociat.Utility
{
    public class Tween
    {
        private MonoBehaviour _runner;
        private IEnumerator _routine;
        private Coroutine _runningCoroutine;
        private Action _onComplete;

        public Tween(MonoBehaviour runner, IEnumerator routine)
        {
            _runner = runner;
            _routine = routine;
        }

        public Tween OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        public void Play()
        {
            _runningCoroutine = _runner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            yield return _runner.StartCoroutine(_routine);
            _onComplete?.Invoke();
        }

        public void Kill()
        {
            if (_runningCoroutine != null)
                _runner.StopCoroutine(_runningCoroutine);
        }
    }

    public class UIFade : MonoBehaviour
    {
        private Dictionary<Image, Tween> activeTweens = new Dictionary<Image, Tween>();
        
        
        public Tween Fade(Image image, float targetAlpha, float duration)
        {
            if (activeTweens.TryGetValue(image, out Tween existingTween))
            {
                existingTween.Kill();
            }
            
            Tween newTween = new Tween(this, FadeCoroutine(image, targetAlpha, duration));
            
            activeTweens[image] = newTween;
            
            newTween.OnComplete(() =>
            {
                if (activeTweens.ContainsKey(image) && activeTweens[image] == newTween)
                    activeTweens.Remove(image);
            });

            return newTween;
        }

        private static IEnumerator FadeCoroutine(Image image, float targetAlpha, float duration)
        {
            float startAlpha = image.color.a;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                Color color = image.color;
                color.a = newAlpha;
                image.color = color;

                yield return null;
            }

            Color finalColor = image.color;
            finalColor.a = targetAlpha;
            image.color = finalColor;
        }
    }
}