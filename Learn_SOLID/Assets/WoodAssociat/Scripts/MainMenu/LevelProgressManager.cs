using System;
using System.Collections;
using UnityEngine;
using WoodAssociat.Utility;

namespace WoodAssociat
{
    public class LevelProgressManager : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float hight;
        [SerializeField] private RectTransform _levelProgressIndicator;
        [SerializeField] private RectTransform[] levelIndicateTransformPoint;
        [SerializeField] private int currentLevelIndex;

        private IEnumerator _levelProgressCoroutine;



        private void Start()
        {
            _levelProgressCoroutine = LevelProgressMoveCoroutine();
        }

        [ContextMenu("Level Progress")]
        public void MoveProgressIndicatorToNextPoint()
        {
            Loghandler.Log("1");
            StartCoroutine(nameof(LevelProgressMoveCoroutine));
        }

        private IEnumerator LevelProgressMoveCoroutine()
        {
            Loghandler.Log("2");
            Transform nextLevelIndicateTransform = GetNextLevelIndicateTransform();

            Vector2 startPoint = _levelProgressIndicator.position;
            Vector2 endPoint = nextLevelIndicateTransform.position;

            float distance = Vector2.Distance(startPoint, endPoint);
            float jounry = 0;

            _levelProgressIndicator.SetParent(nextLevelIndicateTransform, false);

            while (jounry < distance)
            {
                Loghandler.Log("3");
                jounry += speed * Time.deltaTime;
                float t = Mathf.Clamp01(jounry / distance);
                Vector2 targetPos = Vector2.Lerp(startPoint, endPoint, t);
                float arc = 4 * hight * t * (1 - t);
                targetPos.y += arc;

                _levelProgressIndicator.position = targetPos;

                yield return null;
            }

            Loghandler.Log("4");
            _levelProgressIndicator.position = endPoint;
            currentLevelIndex++;
        }

        private Transform GetNextLevelIndicateTransform()
        {
            var nextLevelIndicateIndex =
                currentLevelIndex + 1 >= levelIndicateTransformPoint.Length ? 0 : currentLevelIndex + 1;

            var nextLevelIndicateTransform = levelIndicateTransformPoint[nextLevelIndicateIndex];

            if (nextLevelIndicateTransform != null) return nextLevelIndicateTransform;

            Loghandler.Log("No level indicator found");
            return null;
        }
    }
}