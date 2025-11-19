using UnityEngine;

/// <summary>
/// Singleton manager that holds the crack sprites database reference
/// </summary>
public class CrackEffectManager : MonoBehaviour
{
    public static CrackEffectManager Instance { get; private set; }
    
    [Header("Crack Sprites Database")]
    public CrackSpritesDatabase spritesDatabase;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public CrackSpritesDatabase GetDatabase()
    {
        return spritesDatabase;
    }
}
