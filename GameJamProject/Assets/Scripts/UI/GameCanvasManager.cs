using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

namespace Jam
{
    public class GameCanvasManager : MonoBehaviour
    {
        public GameObject personPrefab; 
        Canvas canvas;
        List<Person> uiPeople;

        public Text infoText; 

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Init()
        {
            canvas = GetComponent<Canvas>();
            uiPeople = new List<Person>();
            infoText.text = "";
        }

        public void SpawnPeople(List<List<PersonTrait>> people)
        {
            Init(); 
            foreach (List<PersonTrait> traits in people)
            {
                GameObject personObj = Instantiate(personPrefab, canvas.gameObject.transform);
                Person person = personObj.GetComponent<Person>();
                person.Traits = new List<PersonTrait>(); 
                foreach (PersonTrait traitData in traits)
                {
                    person.Traits.Add(traitData);
                }
                uiPeople.Add(person); 
            }
            
        }

        public void ShowPersonText(List<PersonTrait> traits)
        {
            infoText.text = "";
            infoText.text += "Character Traits\n"; 
            foreach(PersonTrait trait in traits)
            {
                infoText.text += trait.Phrase;
                infoText.text += "\n"; 
            }
        }
    }
}
