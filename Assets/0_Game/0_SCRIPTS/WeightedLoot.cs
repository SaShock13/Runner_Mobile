using UnityEngine;

public enum LootRarity
{
    Common, Rare, Epic, Legendary
}

[System.Serializable]
public struct WeightedLoot 
{    
    public LootRarity rarity;
    public float weight;
    public GameObject bonusPrefab;
}
