using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jam
{
    public class AreaUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector]
        public RectTransform rect;
        [HideInInspector]
        public List<Person> containedPeople; 

        // Use this for initialization
        void Start() {
            rect = GetComponent<RectTransform>();
            containedPeople = new List<Person>(); 
        }

        // Update is called once per frame
        void Update() {

        }

        //Detect if the Cursor starts to pass over the GameObject
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            //Output to console the GameObject's name and the following message
            //Debug.Log("Cursor Entering " + name + " GameObject");
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            //Output the following message with the GameObject's name
            //Debug.Log("Cursor Exiting " + name + " GameObject");
        }
    }
}
