using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BanWords", menuName = "ScriptableObject/BanWords")]
public class PostItBanWords : ScriptableObject
{
    public List<string> banWords = new List<string>();
}
