public class Module_Zero : GenerationModule
{

    public bool invert = false;

    public override int Sort(ModuleReferenceData data)
    {
        if(!invert) 
            return 0;
        else
            return data.connectors.Count-1;
    }
}
