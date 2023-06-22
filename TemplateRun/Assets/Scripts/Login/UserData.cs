using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/UserData")]
public class UserData : ScriptableObject
{
    public string Nickname = string.Empty;
}
