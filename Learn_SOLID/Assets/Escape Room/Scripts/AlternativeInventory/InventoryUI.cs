using System;
using UnityEngine;


    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI instance;
        
        public InventorySlot[] slots;
        
        public int currentSlot;

        private void Awake()
        {
            instance = this;
        }

        public void AddItem(InventoryItemData slot)
        {
            slots[currentSlot].SetItem(slot);
            currentSlot++;
        }
    }
