using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam
{
    public class GameCanvasManager : MonoBehaviour
    {
        GameObject personPrefab; 
        Canvas canvas;
        List<Person> uiPeople; 
        // Use this for initialization
        void Start()
        {
            canvas = GetComponent<Canvas>();
            uiPeople = new List<Person>(); 
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SpawnPeople(List<List<PersonTrait>> people)
        {
            foreach(List<PersonTrait> traits in people)
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
    }
}
