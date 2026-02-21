using Unity.VisualScripting;
using UnityEngine;

public class Trigger : MonoBehaviour
{
   [SerializeField] private GameObject panel;

   private void OnTriggerEnter2D(Collider2D other)
   {
      panel.SetActive(true);
   }
}
