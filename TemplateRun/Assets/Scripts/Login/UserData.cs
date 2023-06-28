using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "UserData", menuName = "TemplateRun/UserData")]
public class UserData : ScriptableObject
{
    public string Nickname = string.Empty;
}
