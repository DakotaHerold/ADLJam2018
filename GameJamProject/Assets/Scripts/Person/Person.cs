using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jam
{
    public class Person : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        JamGameManager gameManager; 

        public Canvas GameCanvas; 
        [HideInInspector]
        public List<PersonTrait> Traits;

        [HideInInspector]
        public List<Areas> winAreas;

        private TriggerSoundGenerator pickSoundGenerator;
        private TriggerSoundGenerator placeSoundGenerator;

        Image uiImage;
        bool inRange = false;
        private bool active = false;
        public bool Active{
            get { return active; }
            set { 
                active = value;
                if (active)
                    pickSoundGenerator.GenerateSound();
                else 
                    placeSoundGenerator.GenerateSound();
            }
        }
        private AreaUI[] areas;

        private void Awake()
        {
            pickSoundGenerator = (GameObject.Find("PickupSoundGenerator")).GetComponent<TriggerSoundGenerator>();
            placeSoundGenerator = (GameObject.Find("PlaceSoundGenerator")).GetComponent<TriggerSoundGenerator>();
        }

        // Use this for initialization
        void Start()
        {
            Init(); 
        }

        public void Init()
        {
            active = false;
            inRange = false;
            gameManager = FindObjectOfType<JamGameManager>();
            uiImage = GetComponent<Image>();
            areas = FindObjectsOfType<AreaUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0) && inRange)
            {
                Active = !active; 
                if(active == false)
                {
                    gameManager.ClearInfoBox(); 
                    foreach (AreaUI area in areas)
                    {
                        if(uiImage.rectTransform.Overlaps(area.rect))
                        {
                            area.containedPeople.Add(this); 
                        }
                    }
                }
                else
                {
                    gameManager.PlayPersonPickupSound(); 
                    gameManager.UpdatePersonText(Traits); 
                    foreach (AreaUI area in areas)
                    {
                        area.containedPeople.Remove(this);
                    }
                }
            }
            if(active){
                transform.position = Input.mousePosition;
                if (!Input.GetMouseButton(0) && active)
                    Active = !active;
            }
        }

        //Detect if the Cursor starts to pass over the GameObject
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            //Output to console the GameObject's name and the following message
            //Debug.Log("Cursor Entering " + name + " GameObject");
            inRange = true; 
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            //Output the following message with the GameObject's name
            //Debug.Log("Cursor Exiting " + name + " GameObject");
            inRange = false; 
        }

        public static bool Contains(Rect rect1, Rect rect2)
        {

            if ((rect1.position.x <= rect2.position.x) &&
                (rect1.position.x + rect1.size.x) >= (rect2.position.x + rect2.size.x) &&
                (rect1.position.y <= rect2.position.y) &&
                (rect1.position.y + rect1.size.y) >= (rect2.position.y + rect2.size.y))
            {

                return true;
            }
            else
            {

                return false;
            }
        }
    }

    public static class RectTransformExtensions
    {

        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            return a.WorldRect().Overlaps(b.WorldRect());
        }
        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }
    }



}

