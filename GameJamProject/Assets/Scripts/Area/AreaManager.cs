using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private int numTraitsPerArea = 2; // Final needs to be two
        public int NumTraitsPerArea { get { return numTraitsPerArea; } }
        private int numTraitsPerPerson = 3; 
        public int NumTraitsPerPerson { get { return numTraitsPerPerson; } }
        private int numTotalTraitsToGenerate = 20; // Final needs to be 10 or more
        public int NumTotalTraitsToGenerate { get { return numTotalTraitsToGenerate; } }
        private int numAreas = 4; 
        public int NumAreas { get { return numAreas; } }
        private int numPeople = 20; 
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
            //Create a number of empty areas
            AreaData[] results = new AreaData[4];
            List<AreaTrait> areaTraits = new List<AreaTrait>();

            // Constructing Traits 
            for (int iTrait = 0; iTrait < numTotalTraitsToGenerate; ++iTrait)
            {
                 
                AreaTrait newTrait = GenerateTrait(); 

                bool isNew = false; 

                while(!isNew)
                {
                    isNew = true; 
                    foreach(AreaTrait trait in areaTraits)
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


                areaTraits.Add(newTrait);
            }

            for(int iArea = 0; iArea < numAreas; ++iArea)
            {
                AreaData newAreaData;
                newAreaData.areaTraits = new List<AreaTrait>(); 
                
                for(int jTrait = 0; jTrait < numTraitsPerArea; ++jTrait)
                {
                    int randInt = UnityEngine.Random.Range(0, areaTraits.Count); 
                    AreaTrait trait = areaTraits[randInt]; 

                    for(int kTrait = 0; kTrait < newAreaData.areaTraits.Count; ++kTrait)
                    {
                        while(trait.category == newAreaData.areaTraits[kTrait].category)
                        {
                            trait = areaTraits[UnityEngine.Random.Range(0, areaTraits.Count)];
                        }
                    }

                    areaTraits.Remove(areaTraits[randInt]);

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
                List<AreaTrait> usedAreaTraits = new List<AreaTrait>(); 

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
                                if(!usedPhrases.Contains(phrase) && !usedAreaTraits.Contains(trait))
                                {
                                    newAreaData.phrases.Add(phrase);
                                    usedPhrases.Add(phrase);
                                    usedAreaTraits.Add(trait); 
                                }
                            }
                        }
                    }

   
                    System.Random rng = new System.Random(); 
                    newAreaData.phrases = newAreaData.phrases.OrderBy(a => rng.Next()).ToList();

                    //List<string> newPhrases = new List<string>();
                    //int randIndex = UnityEngine.Random.Range(0, newAreaData.phrases.Count);
                    //string randPhrase = newAreaData.phrases[randIndex];
                    //newPhrases.Add(randPhrase); 
                    
                    //newAreaData.phrases = newPhrases; 

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
                    possiblePeopleTraits.Add(trait); 
                }
            }

            // Find common denominator 
            float numPeopleFloat = (((float)numPeople) / ((float)numAreas));
            int numPeoplePerArea = Mathf.CeilToInt(numPeopleFloat);
            //Debug.Log("Num people per area: " + numPeoplePerArea);

            List<PersonTrait> sortedTraits = new List<PersonTrait>();

            // TODO Needs to sort and save category&& group here only. When adding

            foreach(AreaData area in finishedAreas)
            {
                foreach(AreaTrait areaTrait in area.areaTraits)
                {
                    foreach(PersonTrait personTrait in possiblePeopleTraits)
                    {
                        if(areaTrait.category == personTrait.Category)
                        {
                            if(areaTrait.group == Group.NotGroup && personTrait.groupType == Group.NotIndividual && !sortedTraits.Contains(personTrait))
                            {
                                sortedTraits.Add(personTrait);
                            }
                            else if(areaTrait.group == Group.Group && personTrait.groupType == Group.Individual && !sortedTraits.Contains(personTrait))
                            {
                                sortedTraits.Add(personTrait);
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
                        // TODO pull random trait that matches category and group 
                        int randInt = UnityEngine.Random.Range(0, sortedTraits.Count);
                        trait = sortedTraits[randInt];
                        //PersonTrait temp = sortedTraits[randInt];
                        //trait = dataManager.PullPersonTrait(temp.Category, temp.groupType);
                        if (usedCategories.Contains(trait.Category))
                        {
                            passed = false; 
                        }
                        else
                        {
                            personsInfo.Add(trait); 
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