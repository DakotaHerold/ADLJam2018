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
        public Button aButton;
        public Button bButton;
        public Button cButton;
        public Button dButton;

        public Sprite smileyFace;
        public Sprite frownyFace;
        public Sprite neutralFace; 

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
        }

        private void Init()
        {
            areaManager = FindObjectOfType<AreaManager>(); 
            canvas = GetComponent<Canvas>();
            uiPeople = new List<Person>();
            infoText.text = "";
            titleText.text = "Info";

            aButton.onClick.AddListener(AButtonClick);
            bButton.onClick.AddListener(BButtonClick);
            cButton.onClick.AddListener(CButtonClick);
            dButton.onClick.AddListener(DButtonClick);
            submitButton.onClick.AddListener(SubmitButtonClick); 
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
                Person person = personObj.GetComponent<Person>();
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
    }
}
