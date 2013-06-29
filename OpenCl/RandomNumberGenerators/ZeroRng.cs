
namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public sealed class ZeroRng : RandomGenerator
    {
        public ZeroRng()
        { }

        internal sealed class DotNetImpl : DotNetRng
        {
            public DotNetImpl()
            { }

            public void Init(uint u)
            { }

            public void Step()
            { }

            public uint GetUint()
            {
                return 0u;
            }
        }

        internal sealed class OpenClImpl : OpenClRng
        {
            private readonly string Prefix;

            public string TypeName { get { return Prefix + "State"; } }

            public OpenClImpl(string prefix)
            {
                this.Prefix = prefix;
            }

            public string DefineStateAndFunctions()
            {
                return "typedef unsigned " + this.TypeName + "; ";
            }

            public string DeclareState(string stateVar)
            {
                return TypeName + " " + stateVar + "; ";
            }

            public string StepState(string stateVar)
            {
                return stateVar + " = " + stateVar + " + 1; ";
            }

            public string InitState(string stateVar, string seedVar)
            {
                return stateVar + " = 0u;";
            }

            public string StupidInitState(string stateVar, string seedVar)
            {
                return stateVar + " = 0u;";
            }

            public string GetUintVal(string stateVar)
            {
                return "0u";
            }
        }

        public DotNetRng GetDotNetImpl()
        {
            return new DotNetImpl();
        }

        public OpenClRng GetOpenClRng(string localVarPrefix)
        {
            return new OpenClImpl(localVarPrefix);
        }
    }
}
