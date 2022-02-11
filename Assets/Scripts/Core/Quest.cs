using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "IEnumerables Primer/Quest", order = 0)]
public class Quest : ScriptableObject {
    [SerializeField] string[] tasks;

    public IEnumerable<string> getTasks(){
        yield return "Task 1";
        Debug.Log("Do some work");
        yield return "Task 2";
    }
    
}