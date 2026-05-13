using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WoodAssociat.Utility
{
    public static class RVUtility
    {
        #region UIRect

        public static void MoveRect(MonoBehaviour runner, RectTransform rectTransform, Vector2 endPos, float speed)
        {
            runner.StartCoroutine(MoveCoutine(rectTransform, endPos, speed));
        }

        private static IEnumerator MoveCoutine(RectTransform rectTransform, Vector2 endPos, float speed)
        {
            Vector2 startPos = rectTransform.position;
            float distance = Vector2.Distance(startPos, endPos);
            float journey = 0;

            while (journey < distance)
            {
                journey += speed * Time.deltaTime;
                float t = journey / distance;
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            rectTransform.anchoredPosition = endPos;
        }


        public static void FadeImageColor(MonoBehaviour runner, Image image, float targetAlpha, float duration)
        {
            runner.StartCoroutine(FadeCoroutine(image, targetAlpha, duration));
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

        #endregion
    }
}