using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WoodAssociat.Utility;

namespace WoodAssociat
{
    public class WoodSlot : MonoBehaviour
    {
        [SerializeField] private List<WoodTile> woodTiles;

        [SerializeField] private Image slotWinIndicateBgImage;
        [SerializeField] UIFade uiFader;

        [SerializeField] private float moveDuration;
        [SerializeField] private float waveDuration;

        [SerializeField] private int width;

        [SerializeField] private Text familyNameText;


        private void Start()
        {
            Invoke(nameof(WaveMove), 1.5f);
        }

        public void AddWoodTile(WoodTile woodTile)
        {
            woodTiles.Add(woodTile);
        }

        public void RemoveWoodTile(WoodTile woodTile)
        {
            woodTiles.Remove(woodTile);
        }

        public void CheckWon()
        {
            if (woodTiles.Count == 0)
                Loghandler.Log("There is no wood tiles in the wood slot");

            WoodTile firstWoodTile = woodTiles[0];

            for (int i = 1; i < woodTiles.Count; i++)
            {
                if (firstWoodTile.GetWoodFamilyId() != woodTiles[i].GetWoodFamilyId())
                    return;
            }
            
            MakeWoodTilesDraggable();
            
            string woodSlotFamilName = WoodeAssociater.Instance.GetWoodFamilyName(firstWoodTile);

            uiFader.Fade(slotWinIndicateBgImage, 1, 0.5f).OnComplete(() =>
            {
                Loghandler.Log("Wood slot won");
                SetFamilyName(woodSlotFamilName);
            }).Play();
        }

        public void SetFamilyName(string familyName)
        {
            familyNameText.text = familyName;
        }

        private void MakeWoodTilesDraggable()
        {
            foreach (var woodTile in woodTiles)
                woodTile.SetWoodTileDragState(false);
        }

        public void WaveMove()
        {
            StartCoroutine(WaveMoveCoroutine());
        }

        private IEnumerator WaveMoveCoroutine()
        {
            for (int i = 0; i < woodTiles.Count; i++)
            {
                int row = i / width;
                int col = i % width;

                float delay = (row + col) * 0.08f;

                StartCoroutine(MoveCoroutine(woodTiles[i].GetWoodRecTransform(), woodTiles[i].GetGridParent().position,
                    delay));
                yield return new WaitForSeconds(waveDuration);
            }
        }

        private IEnumerator MoveCoroutine(RectTransform obj, Vector2 target, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector2 startPos = obj.anchoredPosition;
            float time = 0f;

            while (time < moveDuration)
            {
                time += Time.deltaTime;
                float t = time / moveDuration;
                t = EaseOutBack(t);

                obj.anchoredPosition = Vector2.LerpUnclamped(startPos, target, t);
                yield return null;
            }

            obj.anchoredPosition = target;
        }

        float EaseOutBack(float t)
        {
            float c1 = 0.5f;
            float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }
    }
}