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

        public static bool operator ==(AreaTrait x, AreaTrait y)
        {
            return x.category == y.category && x.group == y.group;
        }
        public static bool operator !=(AreaTrait x, AreaTrait y)
        {
            return !(x == y);
        }
    }

    public class AreaManager : MonoBehaviour
    {
        public AreaUI[] areaUIs; 

        [HideInInspector]
        private DataManager dataManager;
        [HideInInspector]
        public AreaData[] finishedAreas;
        [HideInInspector]
        public List<List<PersonTrait>> finishedPeople; 

        private int numTraitsPerArea = 1; // Final needs to be two
        public int NumTraitsPerArea { get { return numTraitsPerArea; } }
        private int numTraitsPerPerson = 3; 
        public int NumTraitsPerPerson { get { return numTraitsPerPerson; } }
        private int numTotalTraitsToGenerate = 20; // Final needs to be 10 or more
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
            //Dictionary<AreaTrait, string> phraseDictionary = new Dictionary<AreaTrait, string>();


            //for (int iPhrase = 0; iPhrase < baseData.Length; ++iPhrase)
            //{
            //    AreaTrait phraseTrait;
            //    phraseTrait.category = baseData[iPhrase].Category;
            //    phraseTrait.group = baseData[iPhrase].Group;

            //    phraseDictionary.Add(phraseTrait, baseData[iPhrase].Text); 
            //}



            //Create a number of empty areas
            AreaData[] results = new AreaData[4];
            List<AreaTrait> usedTraits = new List<AreaTrait>();

            // Constructing Traits 
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
                    int randInt = UnityEngine.Random.Range(0, usedTraits.Count-1); 
                    AreaTrait trait = usedTraits[randInt]; 

                    for(int kTrait = 0; kTrait < newAreaData.areaTraits.Count; ++kTrait)
                    {
                        while(trait.category == newAreaData.areaTraits[kTrait].category)
                        {
                            trait = usedTraits[UnityEngine.Random.Range(0, usedTraits.Count-1)];
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

                List<string> usedPhrases = new List<string>(); 

               for(int iTrait = 0; iTrait < newAreaData.areaTraits.Count; ++iTrait)
               {
                    AreaTrait trait = newAreaData.areaTraits[iTrait];

                    string phrase = ""; 
                    while(phrase == "")
                    {
                        foreach(PhraseData phraseInfo in baseData)
                        {
                            if(phraseInfo.Category == trait.category && phraseInfo.Group == trait.group)
                            {
                                phrase = phraseInfo.Text;
                                if(!usedPhrases.Contains(phrase))
                                {
                                    newAreaData.phrases.Add(phrase);
                                    usedPhrases.Add(phrase); 
                                }
                            }
                        }
                    }

                    //newAreaData.phrases.Add(phraseDictionary[trait]); 
                    
               }

                results[iArea] = newAreaData; 
            }

            return results;
        }

        public void ConstructPeople(int numPeople, PhraseData[] baseData)
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

            List<PersonTrait> sortedTraits = new List<PersonTrait>();
            //List<PersonTrait> sortedNegativeTraits = new List<PersonTrait>(); 

            foreach(AreaData area in finishedAreas)
            {
                foreach(AreaTrait areaTrait in area.areaTraits)
                {
                    foreach(PersonTrait personTrait in possiblePeopleTraits)
                    {
                        if(areaTrait.category == personTrait.Category)
                        {
                            if(areaTrait.group == Group.NotGroup && personTrait.groupType == Group.NotIndividual)
                            {
                                sortedTraits.Add(personTrait);
                                break; 
                            }
                            else if(areaTrait.group == Group.Group && personTrait.groupType == Group.Individual)
                            {
                                sortedTraits.Add(personTrait);
                                break; 
                            }
                        }
                    }
                }
            }


            // Divide traits
            List<List<PersonTrait>> finalizedPeople = new List<List<PersonTrait>>(); 
            for(int iPerson = 0; iPerson < numPeople; ++iPerson)
            {
                List<PersonTrait> personsInfo = new List<PersonTrait>();
                List<Category> usedCategories = new List<Category>(); 
                for (int jTrait = 0; jTrait < numTraitsPerPerson; ++jTrait)
                {
                    bool passed = false;
                    PersonTrait trait;
                    while (!passed)
                    {
                        passed = true; 
                        trait = sortedTraits[UnityEngine.Random.Range(0, sortedTraits.Count-1)]; 
                        if(usedCategories.Contains(trait.Category))
                        {
                            passed = false; 
                        }
                        else
                        {
                            usedCategories.Add(trait.Category); 
                        }
                    }
                    
                }
                finalizedPeople.Add(personsInfo); 
            }

            finishedPeople = finalizedPeople; 
            
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