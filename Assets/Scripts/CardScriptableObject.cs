using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewCard", menuName ="Card", order = 1 )]
public class CardScriptableObject : ScriptableObject
{
    public string cardName;

    [TextArea]
    public string actionDescription, cardLore, cardType;
    public int currentHealth, attackPower, manaCost;

    public Sprite chracterSprite, bgSprite;
}
