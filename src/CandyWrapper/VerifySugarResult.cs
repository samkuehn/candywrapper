internal static class VerifySugarResult
{
    internal static void Verify(error_value error)
    {
        if (error.number == "0")
            return;
        throw new SugarException(error);
    }
}