using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using Lucy.Features.Items;
using System;
namespace Lucy.Core.FSM.Graph
{
    public class State2_UseScrewdriver : State
    {
        [Header("Reference")]
        public TMP_Text text;
        public OneHandInventory inventory;
        public Counter screwCounter;
        public Entity player;
        [Header("Runtime")]
        public bool completed = false;
        public bool breaked = false;
        public bool failed = false;

        public override void Enter()
        {
            completed = false;
            breaked = false;
            failed = false;
            screwCounter.OnFinished.AddListener(ScrewCounterFinished);
            inventory.OnItemRemoved += OnItemRemoved;
            player.OnDeath += PlayerDied;
            text.text = "Use Screwdriver";
            Debug.Log("C# Logic: Ожидаем откручивания решетки..."); 
        }

        public override bool Decide()
        {
            return completed || breaked || failed;
        }

        public override IEnumerator Execute()
        {
            while (!Decide())
            {
                Debug.Log("[State2] Execute");
                yield return null;
            }
        }

        public override void Exit()
        {
            StateResult = completed ? "Success" : (breaked ? "Breaked" : "Failed");

            if (inventory != null) inventory.OnItemRemoved -= OnItemRemoved;
            if (player != null) player.OnDeath -= PlayerDied;
            if (screwCounter != null) screwCounter.OnFinished.RemoveListener(ScrewCounterFinished);
            
            Debug.Log("[State2] Exit " + StateResult);
        }
        void PlayerDied(object obj)
        {
            failed = true;
            player.OnDeath -= PlayerDied;
        }

        void OnItemRemoved(ItemInstance instance)
        {
            if (instance.Defination.id == "screwdriver") breaked = true;
            inventory.OnItemRemoved -= OnItemRemoved;
        }

        void ScrewCounterFinished()
        {
            completed = true;
            screwCounter.OnFinished.RemoveListener(ScrewCounterFinished);
        }
    }
}