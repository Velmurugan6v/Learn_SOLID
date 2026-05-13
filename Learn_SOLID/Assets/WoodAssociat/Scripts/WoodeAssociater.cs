using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WoodAssociat
{
    public class WoodeAssociater : MonoBehaviour
    {
        public static WoodeAssociater Instance;
        [SerializeField] float tileArcHeight = 0.15f;
        [SerializeField] private float tileMoveDuration = 0.35f;
        [SerializeField] private float amplitude = 20f;

        [SerializeField] private List<WoodSlot> woodSlots;

        [SerializeField] private List<string> woodFamilyNames;

        private IEnumerator secondTileCoroutine;

        private void Start()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }

        public void SwapTiles(WoodTile tile1, WoodTile tile2)
        {
            StartCoroutine(SwapTilesCoroutine(tile1, tile2));
        }

        private IEnumerator SwapTilesCoroutine(WoodTile tile1, WoodTile tile2)
        {
            //Get Parent
            Transform tile1TransformParent = tile1.GetGridParent();
            Transform tile2TransformParent = tile2.GetGridParent();

            //Remove from currentSlot
            tile1.GetWoodSlot().RemoveWoodTile(tile1);
            tile2.GetWoodSlot().RemoveWoodTile(tile2);

            //Get SlotParent
            WoodSlot tile1WoodSlot = tile1.GetWoodSlot();
            WoodSlot tile2WoodSlot = tile2.GetWoodSlot();

            WoodSlot tempWoodSlot = tile1WoodSlot;

            tile1.SetWoodSlot(tile2WoodSlot);
            tile2.SetWoodSlot(tempWoodSlot);

            //Add new slot
            tile1.GetWoodSlot().AddWoodTile(tile1);
            tile2.GetWoodSlot().AddWoodTile(tile2);

            StartCoroutine(SwapTilesCoroutine(tile1, tile2TransformParent, tileMoveDuration));
            yield return SwapTilesCoroutine(tile2, tile1TransformParent, tileMoveDuration, tileArcHeight);
        }

        private IEnumerator SwapTilesCoroutine(WoodTile tile, Transform parent, float duration, float hight = 0)
        {
            float elapsedTime = 0;

            RectTransform tileTransform = tile.transform as RectTransform;

            Vector2 startPos = tile.transform.position;
            Vector2 endPos = parent.position;

            tileTransform.SetParent(parent, true);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                Vector2 targetPos = Vector2.Lerp(startPos, endPos, t);
                float arc = 4 * hight * t * (1 - t);
                targetPos.y += arc;

                tileTransform.position = targetPos;

                yield return null;
            }

            tileTransform.position = endPos;
            tileTransform.anchoredPosition = Vector2.zero;
            tile.SetGridParent(parent);

            yield return new WaitForSeconds(0.2f);
            tile.GetWoodSlot().CheckWon();
        }

        private IEnumerator MoveSingleWave(WoodTile tile, Transform parent, float speed, float amplitude = 0)
        {
            RectTransform tileTransform = tile.transform as RectTransform;

            Vector2 start = tile.transform.position;
            Vector2 end = parent.position;

            float distance = Vector2.Distance(start, end);
            float journey = 0f;

            Vector2 direction = (end - start).normalized;

            // 🔥 Perpendicular direction for wave
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            while (journey < distance)
            {
                journey += speed * Time.deltaTime;

                float t = Mathf.Clamp01(journey / distance);

                // 🔹 Straight movement
                Vector2 pos = Vector2.Lerp(start, end, t);

                // 🔥 SINGLE WAVE
                float wave = Mathf.Sin(t * Mathf.PI) * amplitude;

                pos += perpendicular * wave;

                tileTransform.position = pos;

                yield return null;
            }

            tileTransform.position = end;
            tileTransform.anchoredPosition = Vector2.zero;
            tile.SetGridParent(parent);
        }

        public string GetWoodFamilyName(WoodTile woodTile)
        {
            int familIdNumber = woodTile.GetWoodFamilyId() - 1;

            return woodFamilyNames[familIdNumber];
        }
    }
}