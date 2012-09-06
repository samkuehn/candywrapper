using System;
using System.Text;

public class SugarException : ApplicationException
{
    private readonly error_value _error;
    private readonly int _pad = 20;

    public SugarException(error_value error)
    {
        _error = error;
    }

    public string Number
    {
        get { return _error.number; }
    }

    public string Description
    {
        get { return _error.description; }
    }

    public string Name
    {
        get { return _error.name; }
    }

    public override string Message
    {
        get { return Description; }
    }

    public override string ToString()
    {
        var error = new StringBuilder();
        error.AppendLine("Name:".PadRight(_pad) + Name);
        error.AppendLine("Description:".PadRight(_pad) + Description);
        error.AppendLine("Number:".PadRight(_pad) + Number);
        return error.ToString();
    }
}