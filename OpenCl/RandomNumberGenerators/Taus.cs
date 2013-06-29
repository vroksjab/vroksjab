using System;

namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    internal sealed class Taus : RandomGenerator
    {
        private readonly int S1;
        private readonly int S2;
        private readonly int S3;
        private readonly uint M;

        private readonly uint InitOrValue;
        private readonly uint InitXorValue;
        private readonly int InitBitRotation;

        // The initOrValue is because the high bits after a step appears independent from the low bits.
        // The bit shifting is because the high bits after a step appears independent from the low bits

        // before the step. Since we normally use 0, 1, 2, 3, .. as initial state and the high bits 
        // are more significant 
        public Taus(int s1, int s2, int s3, uint m, uint initOrValue, uint initXorValue, int initBitRotation)
        {
            if (initOrValue < 256)
            {
                throw new ArgumentException();
            }
            if (initBitRotation >= 32 || initBitRotation < 0)
            {
                throw new ArgumentException();
            }
            this.S1 = s1;
            this.S2 = s2;
            this.S3 = s3;
            this.M = m;
            this.InitOrValue = initOrValue;
            this.InitXorValue = initXorValue;
            this.InitBitRotation = initBitRotation;
        }

        internal sealed class DotNetImpl : DotNetRng
        {
            private readonly Taus Generator;
            private uint z;

            public DotNetImpl(Taus generator)
            {
                this.Generator = generator;
            }

            private void TausStep()
            {
                uint b = ((z << Generator.S1) ^ z) >> Generator.S2;
                z = ((z & Generator.M) << Generator.S3) ^ b;
            }

            public void Init(uint u)
            {
                uint rotatedU = Generator.InitBitRotation == 0 ? u :
                    (u << Generator.InitBitRotation) ^ (u >> (32 - Generator.InitBitRotation));

                z = Generator.InitOrValue | (rotatedU ^ Generator.InitXorValue);
            }

            public void Step()
            {
                TausStep();
            }

            public uint GetUint()
            {
                return z;
            }
        }

        internal sealed class OpenClImpl : OpenClRng
        {
            private readonly Taus Generator;
            private readonly string Prefix;

            public string TypeName { get { return Prefix + "State"; } }

            public OpenClImpl(Taus generator, string prefix)
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

            public string InitState(string stateVar, string seedVar)
            {
                string rotatedSeedVar = "rotate(" + seedVar + ", " + Generator.InitBitRotation + "u)";
                return stateVar + " = " + Generator.InitOrValue + "u | ( " + rotatedSeedVar + " ^ " + Generator.InitXorValue + "u); ";
            }

            public string StupidInitState(string stateVar, string seedVar)
            {
                return stateVar + " = " + Generator.InitOrValue + "u | " + seedVar + "; ";
            }

            public string StepState(string stateVar)
            {
                string z = "(" + stateVar + ")";
                // original code:
                // unsigned b = (((z << S1) ^ z) >> S2);
                // z = (((z & M) << S3) ^ b);

                string b = "((" + z + " << " + Generator.S1 + ") ^ " + z + ") >> " + Generator.S2;
                return z + " = ("
                    + "((" + z + " & " + Generator.M + "u) << " + Generator.S3 + ") ^ " + b
                    + ");  ";
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

        public OpenClRng GetOpenClRng(string prefix)
        {
            return new OpenClImpl(this, prefix);
        }
    }
}
