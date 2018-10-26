using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

namespace Jam
{
    public class GameCanvasManager : MonoBehaviour
    {
        public Transform spawnTransform;
        private Vector3 offsetVector;
        private float offset = 40.0f;

        public GameObject personPrefab; 
        Canvas canvas;
        List<Person> uiPeople;

        public Text titleText; 
        public Text infoText;

        private AreaManager areaManager;

        public Button submitButton;
        public Button creditsButton; 
        public Button aButton;
        public Button bButton;
        public Button cButton;
        public Button dButton;

        public Sprite smileyFace;
        public Sprite frownyFace;
        public Sprite neutralFace;

        public JamGameManager gameManager; 

        private void AButtonClick()
        {
            titleText.text = "Lunch Table A";
            infoText.text = "";
            foreach (string phrase in areaManager.finishedAreas[0].phrases)
            {
                infoText.text += "\n - " + "\"" + phrase + "\"";
                infoText.text += "\n";
            }

            Debug.Log("Table A:\n"); 
            foreach(AreaTrait trait in areaManager.finishedAreas[0].areaTraits)
            {
                Debug.Log("Category: " + trait.category);
                Debug.Log("Group: " + trait.group);
                Debug.Log("\n");
            }
        }

        private void BButtonClick()
        {
            titleText.text = "Lunch Table B";
            infoText.text = "";

            Debug.Log("Table B:\n");
            foreach (string phrase in areaManager.finishedAreas[1].phrases)
            {
                infoText.text += "\n - " + "\"" + phrase + "\"";
                infoText.text += "\n";
            }

            foreach (AreaTrait trait in areaManager.finishedAreas[1].areaTraits)
            {
                Debug.Log("Category: " + trait.category);
                Debug.Log("Group: " + trait.group);
                Debug.Log("\n");
            }
        }

        private void CButtonClick()
        {
            titleText.text = "Lunch Table C";
            infoText.text = "";

            Debug.Log("Table C:\n");
            foreach (string phrase in areaManager.finishedAreas[2].phrases)
            {
                infoText.text += "\n - " + "\"" + phrase + "\"";
                infoText.text += "\n";
            }

            foreach (AreaTrait trait in areaManager.finishedAreas[2].areaTraits)
            {
                Debug.Log("Category: " + trait.category);
                Debug.Log("Group: " + trait.group);
                Debug.Log("\n");
            }
        }

        private void DButtonClick()
        {
            titleText.text = "Lunch Table D";
            infoText.text = "";

            Debug.Log("Table D:\n");
            foreach (string phrase in areaManager.finishedAreas[3].phrases)
            {
                infoText.text += "\n - " + "\"" + phrase + "\"";
                infoText.text += "\n";
            }

            foreach (AreaTrait trait in areaManager.finishedAreas[3].areaTraits)
            {
                Debug.Log("Category: " + trait.category);
                Debug.Log("Group: " + trait.group);
                Debug.Log("\n");
            }
        }

        private void SubmitButtonClick()
        {
            submitButton.onClick.RemoveAllListeners(); 
            submitButton.GetComponentInChildren<Text>().text = "Play Again";
            submitButton.onClick.AddListener(PlayAgainButtonClick); 


            int correct = 0; 
            for(int iArea = 0; iArea < areaManager.areaUIs.Length; ++iArea)
            {
                foreach(Person person in areaManager.areaUIs[iArea].containedPeople)
                {
                    if(person.winAreas.Contains(areaManager.finishedAreas[iArea].name))
                    {
                        correct++;
                        person.GetComponent<Image>().sprite = smileyFace; 
                    }
                    else
                    {
                        person.GetComponent<Image>().sprite = frownyFace; 
                    }
                }
            }

            titleText.text = "Results";
            infoText.text = "You got " + correct + "/" + areaManager.NumPeople + " correct!\n\n";
            infoText.text += "Answer Key:\n";
            infoText.text += "Table A ";
            foreach (AreaTrait trait in areaManager.finishedAreas[0].areaTraits)
            {
                if(trait.group == Group.Group)
                {
                    infoText.text += "Liked " + trait.category.ToString() + " "; 
                }
                else
                {
                    infoText.text += "Disliked " + trait.category.ToString() + " "; 
                }
                if(trait != areaManager.finishedAreas[0].areaTraits[areaManager.finishedAreas[0].areaTraits.Count-1])
                {
                    infoText.text += "and "; 
                }
            }
            infoText.text += "\nTable B ";
            foreach (AreaTrait trait in areaManager.finishedAreas[1].areaTraits)
            {
                if (trait.group == Group.Group)
                {
                    infoText.text += "Liked " + trait.category.ToString() + " ";
                }
                else
                {
                    infoText.text += "Disliked " + trait.category.ToString() + " ";
                }
                if (trait != areaManager.finishedAreas[1].areaTraits[areaManager.finishedAreas[1].areaTraits.Count - 1])
                {
                    infoText.text += "and ";
                }
            }
            infoText.text += "\nTable C ";
            foreach (AreaTrait trait in areaManager.finishedAreas[2].areaTraits)
            {
                if (trait.group == Group.Group)
                {
                    infoText.text += "Liked " + trait.category.ToString() + " ";
                }
                else
                {
                    infoText.text += "Disliked " + trait.category.ToString() + " ";
                }
                if (trait != areaManager.finishedAreas[2].areaTraits[areaManager.finishedAreas[2].areaTraits.Count - 1])
                {
                    infoText.text += "and ";
                }
            }
            infoText.text += "\nTable D ";
            foreach (AreaTrait trait in areaManager.finishedAreas[3].areaTraits)
            {
                if (trait.group == Group.Group)
                {
                    infoText.text += "Liked " + trait.category.ToString() + " ";
                }
                else
                {
                    infoText.text += "Disliked " + trait.category.ToString() + " ";
                }
                if (trait != areaManager.finishedAreas[3].areaTraits[areaManager.finishedAreas[3].areaTraits.Count - 1])
                {
                    infoText.text += "and ";
                }
            }

        }

        private void PlayAgainButtonClick()
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SubmitButtonClick); 
            for (int iArea = 0; iArea < areaManager.areaUIs.Length; ++iArea)
            {
                areaManager.areaUIs[iArea].containedPeople.Clear();
            }
            foreach (Person person in uiPeople)
            {
                person.winAreas = null;
                person.Traits = null;
                person.Active = false; 
                Destroy(person.gameObject); 
            }
            
            uiPeople = new List<Person>();
            areaManager.finishedAreas = null;
            areaManager.finishedPeople = null;
            gameManager.InitializeGame(); 
        }

        private void CreditsButtonClick()
        {
            ClearInfoBox();
            titleText.text = "Project: Lunch Table";
            infoText.text += "Created for ADL Jam 2018\n\nSpecial Thanks to the ADL\n\n";
            infoText.text += "Narrative Design: Brian Holley\n";
            infoText.text += "Sound Design: Forrest Z. Shooster\n";
            infoText.text += "Game Programmer: Chris Grate\n";
            infoText.text += "Game Programmer: Dakota Herold\n";
        }

        private void Init()
        {
            areaManager = FindObjectOfType<AreaManager>(); 
            canvas = GetComponent<Canvas>();
            uiPeople = new List<Person>();
            SetTextToStart(); 

            aButton.onClick.AddListener(AButtonClick);
            bButton.onClick.AddListener(BButtonClick);
            cButton.onClick.AddListener(CButtonClick);
            dButton.onClick.AddListener(DButtonClick);
            submitButton.onClick.AddListener(SubmitButtonClick);
            submitButton.GetComponentInChildren<Text>().text = "Submit";
            creditsButton.onClick.AddListener(CreditsButtonClick); 
        }

        public void SpawnPeople(List<List<PersonTrait>> people)
        {
            Init();

            offsetVector = Vector3.zero;
            int counter = 0;

            offset = 125.0f * canvas.scaleFactor; 
            //Debug.Log("Offset: " + offset);

            foreach (List<PersonTrait> traits in people)
            {
                GameObject personObj = Instantiate(personPrefab, canvas.gameObject.transform);
                personObj.transform.localScale *= 0.85f;
                Person person = personObj.GetComponent<Person>();
                person.Init();
                person.Traits = new List<PersonTrait>(); 
                foreach (PersonTrait traitData in traits)
                {
                    person.Traits.Add(traitData);
                }
                person.winAreas = areaManager.GetPersonSuccessAreas(person);

                //foreach(Areas area in person.winAreas)
                //{
                //    Debug.Log(area);
                //}
                //Debug.Log("\n"); 

                uiPeople.Add(person);

                // Offset spawn
                Vector3 newPos = new Vector3(spawnTransform.GetComponent<RectTransform>().position.x, spawnTransform.GetComponent<RectTransform>().position.y, 0);

                personObj.GetComponent<RectTransform>().position = newPos;
                personObj.GetComponent<RectTransform>().anchoredPosition = spawnTransform.GetComponent<RectTransform>().anchoredPosition;

                personObj.transform.position += offsetVector;
                offsetVector.x += offset;
                counter++;
                if (counter > 4)
                {
                    counter = 0;
                    offsetVector.x = 0.0f;
                    offsetVector.y -= offset;
                }

            }
            
        }

        public void ShowPersonText(List<PersonTrait> traits)
        {
            titleText.text = "Likes and Dislikes";
            infoText.text = ""; 
            foreach(PersonTrait trait in traits)
            {
                infoText.text += "\n - " + "\"" + trait.Phrase + "\"";
                infoText.text += "\n"; 
            }
        }

        public void ClearInfoBox()
        {
            titleText.text = "";
            infoText.text = ""; 
        }

        public void SetTextToStart()
        {
            infoText.text = "Place each person at a table for lunch. Click to pick up and view a person's interests. Click again to place them. Make sure they get along with others at their table or they'll have a bad lunch break! Check a table's interests using the corresponding button on the right. Press submit when you're ready to see how you did!";
            titleText.text = "Project: Lunch Table";
        }
    }
}
