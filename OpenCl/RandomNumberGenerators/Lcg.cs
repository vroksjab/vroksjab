using System;

namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public sealed class Lcg : RandomGenerator
    {
        public readonly uint A;
        public readonly uint C;

        private readonly uint InitOrValue;
        private readonly uint InitXorValue;
        private readonly int InitBitRotation;

        // The initOrValue is because the high bits after a step appears independent from the low bits.
        // The bit shifting is because the high bits after a step appears independent from the low bits

        // before the step. Since we normally use 0, 1, 2, 3, .. as initial state and the high bits 
        // are more significant 
        public Lcg(uint a, uint c, uint initOrValue, uint initXorValue, int initBitRotation)
        {
            if (initBitRotation >= 32 || initBitRotation < 0)
            {
                throw new ArgumentException();
            }
            this.A = a;
            this.C = c;

            this.InitOrValue = initOrValue;
            this.InitXorValue = initXorValue;
            this.InitBitRotation = initBitRotation;
        }

        internal sealed class DotNetImpl : DotNetRng
        {
            private readonly Lcg Generator;
            private uint z;

            public DotNetImpl(Lcg generator)
            {
                this.Generator = generator;
            }

            public void Init(uint u)
            {
                uint rotatedU = (u << Generator.InitBitRotation) ^
                    (u >> (32 - Generator.InitBitRotation));

                z = Generator.InitOrValue | (rotatedU ^ Generator.InitXorValue);
            }

            public void Step()
            {
                z = Generator.A * z + Generator.C;
            }

            public uint GetUint()
            {
                return z;
            }
        }

        internal sealed class OpenClImpl : OpenClRng
        {
            private readonly Lcg Generator;
            private readonly string Prefix;

            public string TypeName { get { return Prefix + "State"; } }

            public OpenClImpl(Lcg generator, string prefix)
            {
                this.Generator = generator;
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
                return stateVar + " = " + stateVar + " * " + Generator.A + "u + " + Generator.C + "u; ";
            }

            public string InitState(string stateVar, string seedVar)
            {
                string rotatedSeedVar = "rotate(" + seedVar + ", " + Generator.InitBitRotation + "u)";
                return stateVar + " = " + Generator.InitOrValue + "u | ( "
                    + rotatedSeedVar + " ^ " + Generator.InitXorValue + "u); ";
            }

            public string StupidInitState(string stateVar, string seedVar)
            {
                return stateVar + " = " + Generator.InitOrValue + "u | " + seedVar + "; ";
            }

            public string GetUintVal(string stateVar)
            {
                return stateVar;
            }
        }

        public DotNetRng GetDotNetImpl()
        {
            return new DotNetImpl(this);
        }

        public OpenClRng GetOpenClRng(string localVarPrefix)
        {
            return new OpenClImpl(this, localVarPrefix);
        }
    }
}
