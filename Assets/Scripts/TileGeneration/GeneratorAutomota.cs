using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorAutomota : MonoBehaviour
{
    Dictionary<string, GenerationModule> modules = new Dictionary<string, GenerationModule>();
    [SerializeField] GenerationModule initialModule;
    public GenerationModule currentModule { get; private set; }

    public void Init()
    {
        //Find all modules
        foreach (Transform child in transform)
        {
            GenerationModule module = child.GetComponent<GenerationModule>();
            if (module != null)
            {
                modules[module.name] = module;
            }
        }

        currentModule = initialModule;
    }

    public void ChangeModule(string moduleName)
    {
        if(currentModule.name == moduleName)
            return;
        
        if(modules.TryGetValue(moduleName, out GenerationModule module))
        {
            currentModule.ExitModule();
            currentModule = module;
            Debug.Log("Found the matching module, switching to: " + moduleName);
        }
    }

    public void ChangeModuleRandom()
    {
        string randomModule = string.Empty;

        //Always return to randomwalk
        if(currentModule.name != "RandomWalk")
        {
            Debug.Log("Changing back to randomwalk!");
            ChangeModule("RandomWalk");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            randomModule = modules.Values.ElementAt(Random.Range(0, modules.Count)).name;
            if (randomModule != currentModule.name)
            {
                break; // Exit the loop if a different module is found, try at most 10 times
            }
        }

        Debug.Log("Randomly changing module to: " + randomModule + " out of " + modules.Count.ToString() + " possibilities");
        ChangeModule(randomModule);
    }

    public List<string> GetAllModuleNames()
    {
        List<string> moduleslist = new List<string>();

        foreach(var module in modules.Values)
        {
            moduleslist.Add(module.name);
        }

        return moduleslist;
    }

}
