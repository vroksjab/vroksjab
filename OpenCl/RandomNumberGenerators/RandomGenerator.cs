namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public interface RandomGenerator
    {
        DotNetRng GetDotNetImpl();
        OpenClRng GetOpenClRng(string localVarPrefix);
    }
}
