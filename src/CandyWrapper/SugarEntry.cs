using System.Collections.Generic;

public class SugarEntry : Dictionary<string, string>
{
    private readonly string _module;

    private SugarEntry()
    {
    }

    public SugarEntry(string module)
    {
        _module = module;
    }

    public string Module
    {
        get { return _module; }
    }

    public string Id
    {
        get
        {
            if (ContainsKey("id"))
                return this["id"];
            return null;
        }
    }
}