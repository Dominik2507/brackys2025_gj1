using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TelepromterHandler : MonoBehaviour
{
    [SerializeField] string file_name = "lorem_ipsum";
    [SerializeField] Transform textPanel;
    [SerializeField] RectTransform inputWordPanel;
    [SerializeField] RectTransform fullTextPanel;
    [Header("Prefabs")]
    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject paragraphPanelPrefab;

   
    [Header("Corruption parameters")]
    [SerializeField] int number_of_words_corrupted = 10;
    [SerializeField] bool use_word_percentage = false;
    [SerializeField] float percent_of_words_corrupted = 0.5f;
    [SerializeField] float character_deletion_chance = 0.5f;



    List<GameObject> word_list;
    int current_highlighted_word = -1;
    private bool allow_input = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        word_list = new List<GameObject>();
        ParseTextDocument("TextFiles/" + file_name);

        RemoveRandomWordsAndCharacters(use_word_percentage, number_of_words_corrupted, percent_of_words_corrupted);

        StartTeleprompterGame();
    }

    private void Update()
    {
        if (allow_input)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                fullTextPanel.gameObject.SetActive(true);
            }
            else if(Input.GetKeyUp(KeyCode.Tab))
            {
                fullTextPanel.gameObject.SetActive(false);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) FindWordAbove();
                if (Input.GetKeyDown(KeyCode.DownArrow)) FindWordBelow();
                if (Input.GetKeyDown(KeyCode.RightArrow)) FindNextWord();
                if (Input.GetKeyDown(KeyCode.LeftArrow)) FindPrevWord();


                for (KeyCode i = KeyCode.Alpha0; i <= KeyCode.Z; i++)
                    if (Input.GetKeyDown(i))
                    {
                        ParseLetterPress(((char)i));
                        break;
                    }
            }
        }
    }

    void ParseLetterPress(char letter)
    {
        CorruptedTeleprompterWord word = word_list[current_highlighted_word].GetComponent<CorruptedTeleprompterWord>();
        string updatedDisplayText = word.display_text;

        for(int i=0; i < word.correct_text.Length; i++)
        {
            if(word.display_text[i] == '_')
            {
                if(char.ToLower(word.correct_text[i]) == letter) updatedDisplayText = word.correct_text.Substring(0, i+1) + updatedDisplayText.Substring(i + 1);
                
                break;
            }
        }

        word.display_text = updatedDisplayText;
        word.GetComponent<TextMeshProUGUI>().text = updatedDisplayText;
        inputWordPanel.GetComponentInChildren<TextMeshProUGUI>().text = updatedDisplayText;

        if (word.display_text == word.correct_text) HandleCorrectedWord();
    }

    void HandleCorrectedWord()
    {
        GameObject word = word_list[current_highlighted_word];

        word.GetComponent<TextMeshProUGUI>().text = word.GetComponent<CorruptedTeleprompterWord>().correct_text;

        Destroy(word.GetComponent<CorruptedTeleprompterWord>());
        
        FindNextWord();
        word.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    void FindNextWord()
    {
        int search_index = (current_highlighted_word + 1) % word_list.Count;
        
        while(search_index != current_highlighted_word)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                UpdateHighlightedWord(search_index);
                break;
            }

            search_index = (search_index + 1) % word_list.Count;
        }
    }

    void FindPrevWord()
    {
        int search_index = (word_list.Count + current_highlighted_word - 1) % word_list.Count;

        while (search_index != current_highlighted_word)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                UpdateHighlightedWord(search_index);
                break;
            }

            search_index = (word_list.Count + search_index - 1) % word_list.Count;
        }
    }

    void FindWordBelow()
    {
        float current_height_pos = word_list[current_highlighted_word].GetComponent<RectTransform>().anchoredPosition.y;
        float target_x = word_list[current_highlighted_word].GetComponent<RectTransform>().anchoredPosition.x;
        float closest_x = -1;
        int closest_index = -1;

        int search_index = (current_highlighted_word + 1) % word_list.Count;
        while (search_index != current_highlighted_word)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                if(current_height_pos != foundWord.GetComponent<RectTransform>().anchoredPosition.y)
                {
                    current_height_pos = foundWord.GetComponent<RectTransform>().anchoredPosition.y;
                    closest_x = foundWord.GetComponent<RectTransform>().anchoredPosition.x;
                    closest_index = search_index;
                    search_index = (search_index + 1) % word_list.Count;
                    break;
                }
            }

            search_index = (search_index + 1) % word_list.Count;
        }

 

        while (current_height_pos == word_list[search_index].GetComponent<RectTransform>().anchoredPosition.y)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                if (Mathf.Abs(target_x - closest_x) > Mathf.Abs(target_x - foundWord.GetComponent<RectTransform>().anchoredPosition.x))
                {
                    closest_index = search_index;
                    closest_x = foundWord.GetComponent<RectTransform>().anchoredPosition.x;
                }
            }

            search_index = (search_index + 1) % word_list.Count;
        }

        UpdateHighlightedWord(closest_index);
    }

    void FindWordAbove()
    {
        float current_height_pos = word_list[current_highlighted_word].GetComponent<RectTransform>().anchoredPosition.y;
        float target_x = word_list[current_highlighted_word].GetComponent<RectTransform>().anchoredPosition.x;
        float closest_x = -1;
        int closest_index = -1;

        int search_index = (word_list.Count + current_highlighted_word - 1) % word_list.Count;

        while (search_index != current_highlighted_word)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                if (current_height_pos != foundWord.GetComponent<RectTransform>().anchoredPosition.y)
                {
                    current_height_pos = foundWord.GetComponent<RectTransform>().anchoredPosition.y;
                    closest_x = foundWord.GetComponent<RectTransform>().anchoredPosition.x;
                    closest_index = search_index;
                    search_index = (word_list.Count + search_index - 1) % word_list.Count;
                    break;
                }
            }

            search_index = (word_list.Count + search_index - 1) % word_list.Count;
        }



        while (current_height_pos == word_list[search_index].GetComponent<RectTransform>().anchoredPosition.y)
        {
            if (word_list[search_index].TryGetComponent(out CorruptedTeleprompterWord foundWord))
            {
                if (Mathf.Abs(target_x - closest_x) > Mathf.Abs(target_x - foundWord.GetComponent<RectTransform>().anchoredPosition.x))
                {
                    closest_index = search_index;
                    closest_x = foundWord.GetComponent<RectTransform>().anchoredPosition.x;
                }
            }

            search_index = (word_list.Count + search_index - 1) % word_list.Count;
        }

        UpdateHighlightedWord(closest_index);
    }

    void UpdateHighlightedWord(int new_index)
    {
        if(current_highlighted_word >= 0)
        {
            word_list[current_highlighted_word].GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        word_list[new_index].GetComponent<TextMeshProUGUI>().color = Color.green;
        inputWordPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(word_list[new_index].GetComponent<CorruptedTeleprompterWord>().display_text);
        current_highlighted_word = new_index;
    }

    void ParseTextDocument(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);

        if (textAsset == null)
        {
            Debug.LogError("Text file not found in Resources: " + path);
            return;
        }

        fullTextPanel.GetComponentInChildren<TextMeshProUGUI>().text = textAsset.text;
        string[] paragraphs = textAsset.text.Split(new string[] { "\n\n", "\r\n\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach(string paragraph in paragraphs)
        {
            GameObject paragraph_panel = Instantiate(paragraphPanelPrefab, textPanel);

            string[] words = paragraph.Split(new char[] { ' ', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach(string word in words)
            {
                GameObject spawnedWord = Instantiate(textPrefab, paragraph_panel.transform);
                spawnedWord.GetComponent<TextMeshProUGUI>().SetText(word);
                word_list.Add(spawnedWord);
            }
        }
    }

    void RemoveRandomWordsAndCharacters(bool use_percentage, int n_of_words, float p_of_words)
    {
        List<int> chosen_words = new List<int>();

        int corrupted_target = n_of_words;
        if (use_percentage) corrupted_target = Mathf.RoundToInt(word_list.Count * p_of_words);
      
        while(chosen_words.Count < corrupted_target)
        {
            int i_word = Random.Range(0, word_list.Count);

            if (!chosen_words.Contains(i_word)) chosen_words.Add(i_word);
        }

        foreach(int index in chosen_words)
        {
            GameObject text_object = word_list[index];
            CorruptedTeleprompterWord corruptedWord = text_object.AddComponent<CorruptedTeleprompterWord>();
            corruptedWord.correct_text = text_object.GetComponent<TextMeshProUGUI>().text;
            corruptedWord.display_text = CorruptWord(corruptedWord.correct_text);
            text_object.GetComponent<TextMeshProUGUI>().text = corruptedWord.display_text;
            text_object.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }

    string CorruptWord(string word)
    {
        char[] corrupted_word = word.ToCharArray();

        for(int i = 0; i < word.Length; i++)
        {
            if (char.IsLetterOrDigit(word[i]) == false) continue;
            if(Random.Range(0, 1.0f) < character_deletion_chance)
            {
                corrupted_word[i] = '_';
            }
        }

        if (string.Compare(word, corrupted_word.ArrayToString()) == 0) return CorruptWord(word);
        return corrupted_word.ArrayToString();
    }

    void StartTeleprompterGame()
    {
        int i = 0;
        foreach(GameObject word in word_list)
        {
            if (word.TryGetComponent(out CorruptedTeleprompterWord cor_word))
            {
                UpdateHighlightedWord(i);
                break;
            }
            i++;
        }

        allow_input = true;
    }
}
