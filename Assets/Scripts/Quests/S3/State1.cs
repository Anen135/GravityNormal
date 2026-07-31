using System;
using System.Collections;
using Lucy.Features.Items;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
namespace Lucy.Core.FSM.Graph
{
    public class State1_FindScrewdriver : State
    {
        [Header("Reference")]
        public TMP_Text text;
        public OneHandInventory inventory;
        public Entity player;
        [Header("Runtime")]
        public bool completed = false;
        public bool failed = false;
        public override void Enter()
        {
            completed = false;
            failed = false;
            text.text = "Find Screwdriver";
            inventory.OnItemAdded += ScrewdriverFound;
            player.OnDeath += PlayerDied;
            Debug.Log("[State1] Enter");
        }

        public override IEnumerator Execute()
        {
            while (!Decide()) {
                Debug.Log("[State1] Execute");
                yield return null;
            }
        }

        public override bool Decide()
        {
            return completed || failed;
        }

        public override void Exit()
        {
            StateResult = completed ? "Success" : "Failed";
            if (inventory != null) inventory.OnItemAdded -= ScrewdriverFound;
            if (player != null) player.OnDeath -= PlayerDied;
            
            Debug.Log("[State1] Exit " + StateResult);
        }
        void PlayerDied(object obj)
        {
            failed = true;
            player.OnDeath -= PlayerDied;
        }

        void ScrewdriverFound(ItemInstance instance)
        {
            if (instance.Defination.id != "screwdriver") return;
            completed = true;
            inventory.OnItemAdded -= ScrewdriverFound;
        }
    }
}