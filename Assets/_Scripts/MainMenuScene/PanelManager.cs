using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
   [SerializeField] private Button button;
   [SerializeField] private GameObject panel;


   public void OnPressEnter()
   {
      panel.SetActive(true);
   }

   public void OnPressExit()
   {
      panel.SetActive(false);
   }



}
