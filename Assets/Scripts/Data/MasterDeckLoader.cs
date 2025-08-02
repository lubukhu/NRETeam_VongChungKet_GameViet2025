// [tooltips] Nạp các thẻ bài từ danh sách cố định vào các ScriptableList Master Deck khi game bắt đầu.
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

public class MasterDeckLoader : MonoBehaviour
{
    [System.Serializable]
    public class DeckDefinition
    {
        public string deckName;
        public List<CardData> sourceCards;
        public ScriptableListCardData targetMasterDeck;
    }

    [SerializeField] private List<DeckDefinition> decksToLoad;
    
    [Header("Output Events")]
    [SerializeField] private ScriptableEventNoParam onMasterDecksLoaded;

    private void Start()
    {
        LoadAllDecks();
    }

    private void LoadAllDecks()
    {
        foreach (var deckDef in decksToLoad)
        {
            if (deckDef.targetMasterDeck != null)
            {
                deckDef.targetMasterDeck.Clear();
                if (deckDef.sourceCards != null && deckDef.sourceCards.Count > 0)
                {
                    deckDef.targetMasterDeck.AddRange(deckDef.sourceCards);
                }
            }
        }
        
        if (onMasterDecksLoaded != null)
        {
            onMasterDecksLoaded.Raise();
        }
    }
}