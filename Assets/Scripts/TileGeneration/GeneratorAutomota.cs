using System.Collections.Generic;
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
        if(modules.TryGetValue(moduleName, out GenerationModule module))
        {
            currentModule.ExitModule();
            currentModule = module;
            Debug.Log("Found the matching module, switching to: " + moduleName);
        }
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
