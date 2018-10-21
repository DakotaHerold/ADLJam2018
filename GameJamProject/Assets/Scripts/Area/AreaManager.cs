using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jam
{
    public struct AreaData
    {
        public Areas name;
        public List<AreaTrait> areaTraits; 
        public List<string> phrases;
    }

    public struct AreaTrait
    {
        public Category category;
        public Group group; 
    }

    public class AreaManager : MonoBehaviour
    {
        public AreaUI[] areaUIs; 

        [HideInInspector]
        private DataManager dataManager;
        [HideInInspector]
        public AreaData[] finishedAreas;
        [HideInInspector]
        public List<PersonTrait> finishedPersonTraits;
        [HideInInspector]
        public List<List<PersonTrait>> finishedPeople; 

        private int numTraitsPerArea = 1; // Final needs to be two
        public int NumTraitsPerArea { get { return numTraitsPerArea; } }
        private int numTotalTraitsToGenerate = 2; // Final needs to be 10 or more
        public int NumTotalTraitsToGenerate { get { return numTotalTraitsToGenerate; } }
        private int numAreas = 4; 
        public int NumAreas { get { return numAreas; } }
        private int numPeople; 
        public int NumPeople { get { return numPeople; } }

        private AreaTrait GenerateTrait()
        {
            AreaTrait newTrait;
            Array values = Enum.GetValues(typeof(Category));
            Category newCat = (Category)values.GetValue(UnityEngine.Random.Range(0, values.Length));

            Group newGroup;
            int randInt = UnityEngine.Random.Range(0, 2);
            if (randInt == 0)
            {
                newGroup = Group.Group;
            }
            else
            {
                newGroup = Group.NotGroup;
            }

            newTrait.category = newCat;
            newTrait.group = newGroup;

            return newTrait; 
        }

        public AreaData[] ConstructArea( PhraseData [] baseData)
        {
            Dictionary<AreaTrait, string> phraseDictionary = new Dictionary<AreaTrait, string>();

            for (int iPhrase = 0; iPhrase < baseData.Length; ++iPhrase)
            {
                AreaTrait phraseTrait;
                phraseTrait.category = baseData[iPhrase].Category;
                phraseTrait.group = baseData[iPhrase].Group;

                phraseDictionary.Add(phraseTrait, baseData[iPhrase].Text); 
            }



            //Create a number of empty areas
            AreaData[] results = new AreaData[4];
            List<AreaTrait> usedTraits = new List<AreaTrait>();

            // Constructing Traits - 
            for (int iTrait = 0; iTrait < numTotalTraitsToGenerate; ++iTrait)
            {
                 
                AreaTrait newTrait = GenerateTrait(); 

                bool isNew = false; 

                while(!isNew)
                {
                    isNew = true; 
                    foreach(AreaTrait trait in usedTraits)
                    {
                        if(newTrait.category == trait.category && newTrait.group == trait.group)
                        {
                            isNew = false;
                            break; 
                        }
                    }
                    if(isNew == false)
                    {
                        newTrait = GenerateTrait(); 
                    }
                }


                usedTraits.Add(newTrait);
            }

            for(int iArea = 0; iArea < numAreas; ++iArea)
            {
                AreaData newAreaData;
                newAreaData.areaTraits = new List<AreaTrait>(); 
                
                for(int jTrait = 0; jTrait < numTraitsPerArea; ++jTrait)
                {
                    int randInt = UnityEngine.Random.Range(0, usedTraits.Count); 
                    AreaTrait trait = usedTraits[randInt]; 

                    for(int kTrait = 0; kTrait < newAreaData.areaTraits.Count; ++kTrait)
                    {
                        while(trait.category == newAreaData.areaTraits[kTrait].category)
                        {
                            trait = usedTraits[UnityEngine.Random.Range(0, usedTraits.Count)];
                        }
                    }

                    usedTraits.Remove(usedTraits[randInt]);

                    newAreaData.areaTraits.Add(trait); 
                }

               if(iArea == 0)
               {
                    newAreaData.name = Areas.One; 
               }
               else if(iArea == 1)
               {
                    newAreaData.name = Areas.Two; 
               }
               else if(iArea == 2)
               {
                    newAreaData.name = Areas.Three; 
               }
               else
               {
                    newAreaData.name = Areas.Four; 
               }

                newAreaData.phrases = new List<string>(); 
               foreach(AreaTrait trait in newAreaData.areaTraits)
                {
                    newAreaData.phrases.Add(phraseDictionary[trait]); 
                }

                results[iArea] = newAreaData; 
            }

            return results;
        }

        public List<PersonTrait> ConstructPeople(int numPeople, PhraseData[] baseData)
        {
            List<PersonTrait> possiblePeopleTraits = new List<PersonTrait>(); 
            foreach(PhraseData phrase in baseData)
            {
                if(phrase.Group == Group.Individual || phrase.Group == Group.NotIndividual)
                {
                    PersonTrait trait;
                    trait.Category = phrase.Category;
                    trait.groupType = phrase.Group;
                    trait.Phrase = phrase.Text; 
                }
            }

            // Find common denominator 
            int numPeoplePerArea = Mathf.CeilToInt(numPeople / numAreas);
            Debug.Log("Num people per area: " + numPeoplePerArea);

            List<PersonTrait> results = new List<PersonTrait>(); 

            foreach(AreaData area in finishedAreas)
            {
                foreach(AreaTrait areaTrait in area.areaTraits)
                {
                    foreach(PersonTrait person in possiblePeopleTraits)
                    {
                        if(areaTrait.category == person.Category)
                        {
                            if(areaTrait.group == Group.NotGroup && person.groupType == Group.NotIndividual)
                            {
                                results.Add(person);
                                break; 
                            }
                            else if(areaTrait.group == Group.Group && person.groupType == Group.Individual)
                            {
                                results.Add(person);
                                break; 
                            }
                        }
                    }
                }
            }


            return results; 
        }

        public List<Areas> GetPersonSuccessAreas(Person person)
        {
            List<Areas> results = new List<Areas>();

            foreach (AreaData area in finishedAreas)
            {
                foreach (AreaTrait areaTrait in area.areaTraits)
                {
                    foreach (PersonTrait personTrait in person.Traits)
                    {
                        if (personTrait.Category == areaTrait.category)
                        {
                            if (areaTrait.group == Group.NotGroup && personTrait.groupType == Group.NotIndividual)
                            {
                                results.Add(area.name); 
                            }
                            else if (areaTrait.group == Group.Group && personTrait.groupType == Group.Individual)
                            {
                                results.Add(area.name);
                            }
                        }
                    }
                }
            }

            return results;
        }

        // Use this for initialization
        void Start()
        {
            dataManager = GetComponent<DataManager>();
            // Testing
            //finishedAreas = ConstructArea(dataManager.phrases); 

            //if(finishedAreas != null)
            //{
            //    foreach(AreaData data in finishedAreas)
            //    {
            //        Debug.Log(data.phrases[0]); 
            //    }
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}