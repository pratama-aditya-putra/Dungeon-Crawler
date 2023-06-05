using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    public bool ignoreRule;
    public float ignoreRulePercentage = 0.3f;

    public List<string> getSentence(string inputString, int iterationLimit)
    {
        List<string> result = new List<string>();
        result.Add(inputString);
        string tempString = inputString;
        for (int i = 0; i < iterationLimit; i++)
        {
            tempString = generateSentence(tempString);
            result.Add(tempString);
        }
        return result;
    }
    public string generateSentence(string inputString)
    {
        if (inputString == null)
            return rootSentence;
        StringBuilder newSentence = new StringBuilder();
        foreach (var c in inputString)
        {
            bool flag = false;
            foreach (Rule rule in rules)
            {
                if((Random.value < ignoreRulePercentage) && ignoreRule)
                {
                    break;
                }
                if(rule.symbol == c.ToString())
                {
                    newSentence.Append(rule.getResult());
                    flag = true;
                }
            }
            if(!flag)
                newSentence.Append(c);
        }
        return newSentence.ToString();
    }
}
