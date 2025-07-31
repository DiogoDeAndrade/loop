using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CharacterArchetype _selectedCharacter;

    static GameManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    static public CharacterArchetype selectedCharacter => instance._selectedCharacter;
}
