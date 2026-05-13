using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WoodAssociat.Utility;

namespace WoodAssociat
{
    public class WoodTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        [Header("Move Details")] [SerializeField]
        private int woodFamilyId = -1;

        private RectTransform _rectTransform;
        [SerializeField] private RectTransform _dragParent;
        private Transform _gridParent;
        private Vector2 _velocity = Vector3.zero;
        private Vector2 _targetPosition;
        private Vector2 _offsetPosition;
        public float yPosition;
        private float _dragMooth = 0.03f;
        private float _speed = 10f;
        private bool canDrag = false;
        private bool _hasDragable = true;

        public Image glowImage;
        public UIFade UIFader;

        [SerializeField] private WoodSlot currentWoodSlot;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            _rectTransform = GetComponent<RectTransform>();
            _gridParent = transform.parent;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_hasDragable) return;

            canDrag = true;
            SetParent();
            SetInitialDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_hasDragable) return;

            canDrag = false;
            DetectTile(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag || !_hasDragable) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragParent, eventData.position,
                eventData.pressEventCamera, out var localPosition);

            _targetPosition = _offsetPosition + localPosition;
            _targetPosition.y += yPosition;
        }

        private void Update()
        {
            if (!canDrag || !_hasDragable) return;

            _rectTransform.anchoredPosition = Vector2.SmoothDamp(_rectTransform.anchoredPosition, _targetPosition,
                ref _velocity, _dragMooth);
        }

        public int GetWoodFamilyId()
        {
            return woodFamilyId;
        }

        private void SetParent()
        {
            Vector3 worldPosition = _rectTransform.position;
            _rectTransform.SetParent(_dragParent, false);
            _rectTransform.position = worldPosition;
        }

        private void MoveToParent()
        {
            StartCoroutine(MoveToParentCoroutine(_gridParent));
        }

        private void DetectTile(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject == gameObject)
                    continue;

                var otherTile = result.gameObject.GetComponent<WoodTile>();

                if (otherTile != null)
                {
                    print(otherTile.name);
                    WoodeAssociater.Instance.SwapTiles(this, otherTile);
                    return;
                }
            }

            MoveToParent();
        }

        private IEnumerator MoveToParentCoroutine(Transform parent)
        {
            Vector2 startPosition = _rectTransform.position;
            Vector2 endPosition = parent.position;

            float distance = Vector2.Distance(startPosition, endPosition);
            float speed = _speed;

            float journey = 0;

            _rectTransform.SetParent(parent, true);

            while (journey < distance)
            {
                journey += speed * Time.deltaTime;
                float t = Mathf.Clamp01(journey / distance);
                _rectTransform.position = Vector2.Lerp(startPosition, endPosition, t);

                yield return null;
            }

            _rectTransform.position = endPosition;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        private IEnumerator MoveUpCoroutine(float yPosition)
        {
            Vector2 targetPosition = _rectTransform.anchoredPosition;
            targetPosition.y += yPosition;

            float speed = 0.2f;
            float elapsedTime = 0;

            while (elapsedTime < speed)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / speed);
                _rectTransform.anchoredPosition = Vector3.Lerp(targetPosition, targetPosition, t);

                yield return null;
            }

            _rectTransform.anchoredPosition = targetPosition;
        }

        private void SetInitialDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragParent, eventData.position,
                eventData.pressEventCamera, out var localPosition);

            _offsetPosition = _rectTransform.anchoredPosition - localPosition;
            _targetPosition = _offsetPosition + localPosition;
            _targetPosition.y += yPosition;
        }

        public Transform GetGridParent()
        {
            return _gridParent;
        }

        public void SetGridParent(Transform parent)
        {
            _gridParent = parent;
        }

        public WoodSlot GetWoodSlot()
        {
            return currentWoodSlot;
        }

        public void SetWoodSlot(WoodSlot woodSlot)
        {
            currentWoodSlot = woodSlot;
        }

        public void SetWoodTileDragState(bool dragValue)
        {
            _hasDragable = dragValue;
        }

        public RectTransform GetWoodRecTransform()
        {
            return _rectTransform;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (canDrag) return;

            if (!_hasDragable) return;

            UIFader.Fade(glowImage, 1f, 0.1f).OnComplete(() => Loghandler.Log("FadeIn Done")).Play();
            //StartCoroutine(MoveUpCoroutine(yPosition));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (canDrag) return;

            if (!_hasDragable) return;

            UIFader.Fade(glowImage, 0f, 0.2f).OnComplete(() => Loghandler.Log("FadeOut Done")).Play();
            //StartCoroutine(MoveUpCoroutine(-yPosition));
        }
    }
}