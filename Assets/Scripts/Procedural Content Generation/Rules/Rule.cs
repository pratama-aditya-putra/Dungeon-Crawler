using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralDungeon/Rule")]
public class Rule : ScriptableObject
{
    public string symbol;
    [SerializeField]
    private string[] result;

    public string getResult()
    {
        return result[Random.Range(0,result.Length)];
    }
}
