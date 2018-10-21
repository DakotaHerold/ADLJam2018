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
        public DataManager dataManager;
        private AreaData[] finishedAreas; 

        private int numTraitsPerArea = 1; // Final needs to be two
        private int numTotalTraitsToGenerate = 2; // Final needs to be 10 or more
        private int NumAreas = 4; 

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

            for(int iArea = 0; iArea < NumAreas; ++iArea)
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

        // Use this for initialization
        void Start()
        {
            dataManager = GetComponent<DataManager>();
            finishedAreas = ConstructArea(dataManager.phrases); 

            if(finishedAreas != null)
            {
                foreach(AreaData data in finishedAreas)
                {
                    Debug.Log(data.phrases[0]); 
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}