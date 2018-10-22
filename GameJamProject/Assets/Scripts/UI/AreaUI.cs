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

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        // Use this for initialization
        void Start() {
            rect = GetComponent<RectTransform>();
            containedPeople = new List<Person>(); 
        }

        // Update is called once per frame
        void Update() {

        }

    }
}
