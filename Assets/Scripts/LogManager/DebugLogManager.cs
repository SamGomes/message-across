using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Debug log manager
public class DebugLogManager : LogManager
{

    public string StringifyDictionaryForLogs(Dictionary<string, string> dict)
    {
        string result = "{";
        List<string> dictKeys = new List<string>(dict.Keys);
        for (int keyI = 0; keyI < dictKeys.Count; keyI++)
        {
            string key = dictKeys[keyI];

            result += " " + key + ": \"" + dict[key] + "\"";
            if (keyI < dictKeys.Count - 1)
            {
                result += ",";
            }
            else
            {
                result += " }";

            }
        }
        return result;
    }

    public override void InitLogs(MonoBehaviour monoBehaviourObject)
    {
        Debug.Log("Log Initialzed.");
    }
    public override IEnumerator WriteToLog(string database, string table, Dictionary<string, string> argsNValues) {
        Debug.Log("database: " + database + " ; " + table + " ; " + StringifyDictionaryForLogs(argsNValues));
        yield return null;
    }


    public override IEnumerator GetFromLog(string database, string table, Func<string, int> yieldedReactionToGet) {
        Debug.Log("database: " + database + " ; " + table + " ; ");
        yield return null;
    }

    public override IEnumerator EndLogs()
    {
        Debug.Log("Log Closed.");
        yield return null;
    }
}

