using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "scriptable_list_" + nameof(CardData), menuName = "Soap/ScriptableLists/"+ nameof(CardData))]
public class ScriptableListCardData : ScriptableList<CardData>
{
    
}
