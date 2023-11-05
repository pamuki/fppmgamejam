using Invector;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Invector.Utils
{
    /// <summary>
    /// Simple Checkpoint Example, works by updating the vGameController SpawnPoint to this transform position/rotation.
    /// </summary>    
    [RequireComponent(typeof(BoxCollider))]
    public class metrolekeso : MonoBehaviour
    {
        vGameController gm;

        public GameObject metro;

        public UnityEvent onTriggerEnter;

        void Start()
        {
            gm = GetComponentInParent<vGameController>();
            this.GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                metro.gameObject.SetActive(true);
                vHUDController.instance.ShowText("Lekésted!");
                gm.spawnPoint = this.gameObject.transform;
                onTriggerEnter.Invoke();
                this.gameObject.SetActive(false);
            }
        }
    }
}


