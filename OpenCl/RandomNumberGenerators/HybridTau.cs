using System;

namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    internal sealed class HybridTau : RandomGenerator
    {
        public readonly Taus Taus1;
        public readonly Taus Taus2;
        public readonly Taus Taus3;
        public readonly Lcg Lcg;

        public HybridTau(Taus taus1, Taus taus2, Taus taus3, Lcg lcg)
        {
            this.Taus1 = taus1;
            this.Taus2 = taus2;
            this.Taus3 = taus3;
            this.Lcg = lcg;
        }

        public DotNetRng GetDotNetImpl()
        {
            return new DotNetImpl(this);
        }

        public OpenClRng GetOpenClRng(string prefix)
        {
            return new OpenClImpl(this, prefix);
        }


        internal sealed class DotNetImpl : DotNetRng
        {
            private readonly HybridTau Generator;

            private readonly DotNetRng Tau1;
            private readonly DotNetRng Tau2;
            private readonly DotNetRng Tau3;
            private readonly DotNetRng Lcg;

            public DotNetImpl(HybridTau generator)
            {
                this.Generator = generator;
                this.Tau1 = generator.Taus1.GetDotNetImpl();
                this.Tau2 = generator.Taus2.GetDotNetImpl();
                this.Tau3 = generator.Taus3.GetDotNetImpl();
                this.Lcg = generator.Lcg.GetDotNetImpl();
            }

            public void Init(uint u)
            {
                /*
                this.Tau1.Init(u);
                this.Tau2.Init(u);
                this.Tau3.Init(u);
                this.Lcg.Init(u);

                /*/
                this.Lcg.Init(u * 8645457);
                this.Lcg.Step();
                this.Lcg.Step();
                this.Lcg.Step();

                this.Tau1.Init(this.Lcg.GetUint());
                this.Lcg.Step();
                this.Tau2.Init(this.Lcg.GetUint());
                this.Lcg.Step();
                this.Tau3.Init(this.Lcg.GetUint());
                this.Lcg.Step();
                // */
            }

            public void Step()
            {
                this.Tau1.Step();
                this.Tau2.Step();
                this.Tau3.Step();
                this.Lcg.Step();
            }

            public uint GetUint()
            {
                return this.Tau1.GetUint() ^ this.Tau2.GetUint() ^ this.Tau3.GetUint() ^ this.Lcg.GetUint();
            }
        }

        internal sealed class OpenClImpl : OpenClRng
        {
            private readonly HybridTau Generator;
            private readonly string Prefix;
            private readonly OpenClRng Tau1;
            private readonly OpenClRng Tau2;
            private readonly OpenClRng Tau3;
            private readonly OpenClRng Lcg;

            public string TypeName { get { return Prefix + "State"; } }

            public OpenClImpl(HybridTau generator, string prefix)
            {
                this.Generator = generator;
                this.Prefix = prefix;
                Tau1 = Generator.Taus1.GetOpenClRng(Prefix + "Tau1");
                Tau2 = Generator.Taus2.GetOpenClRng(Prefix + "Tau2");
                Tau3 = Generator.Taus3.GetOpenClRng(Prefix + "Tau3");
                Lcg = Generator.Lcg.GetOpenClRng(Prefix + "Lcg");
            }

            public string DefineStateAndFunctions()
            {
                return
                Tau1.DefineStateAndFunctions() + Environment.NewLine
                + Tau2.DefineStateAndFunctions() + Environment.NewLine
                + Tau3.DefineStateAndFunctions() + Environment.NewLine
                + Lcg.DefineStateAndFunctions() + Environment.NewLine
                + "typedef struct { "
                    + Tau1.TypeName + " z1; "
                    + Tau2.TypeName + " z2; "
                    + Tau3.TypeName + " z3; "
                    + Lcg.TypeName + " z4;"
                    + " } " + this.TypeName + "; ";
            }

            public string DeclareState(string stateVar)
            {
                return TypeName + " " + stateVar + "; ";
            }

            public string InitState(string stateVar, string seedVar)
            {
                /*
                return
                    Tau1.InitState(stateVar + ".z1", seedVar) + Environment.NewLine
                    + Tau2.InitState(stateVar + ".z2", seedVar) + Environment.NewLine
                    + Tau3.InitState(stateVar + ".z3", seedVar) + Environment.NewLine
                    + Lcg.InitState(stateVar + ".z4", seedVar);
                /*/
                return Lcg.InitState(stateVar + ".z4", seedVar + " * 8645457") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine

                    + Tau1.InitState(stateVar + ".z1", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Tau2.InitState(stateVar + ".z2", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Tau3.InitState(stateVar + ".z3", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    ;
                 // */
            }

            public string StupidInitState(string stateVar, string seedVar)
            {
                //return
                //    Tau1.StupidInitState(stateVar + ".z1", seedVar) + Environment.NewLine
                //    + Tau2.StupidInitState(stateVar + ".z2", seedVar) + Environment.NewLine
                //    + Tau3.StupidInitState(stateVar + ".z3", seedVar) + Environment.NewLine
                //    + Lcg.StupidInitState(stateVar + ".z4", seedVar);

                return Lcg.StupidInitState(stateVar + ".z4", seedVar) + Environment.NewLine

                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine

                    + Tau1.StupidInitState(stateVar + ".z1", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Tau2.StupidInitState(stateVar + ".z2", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    + Tau3.StupidInitState(stateVar + ".z3", Lcg.GetUintVal(stateVar + ".z4")) + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4") + Environment.NewLine
                    ;
            }

            public string StepState(string stateVar)
            {
                return
                    Tau1.StepState(stateVar + ".z1") + Environment.NewLine
                    + Tau2.StepState(stateVar + ".z2") + Environment.NewLine
                    + Tau3.StepState(stateVar + ".z3") + Environment.NewLine
                    + Lcg.StepState(stateVar + ".z4");
            }

            public string GetUintVal(string stateVar)
            {
                return
                    Tau1.GetUintVal(stateVar + ".z1") + " ^ "
                    + Tau2.GetUintVal(stateVar + ".z2") + " ^ "
                    + Tau3.GetUintVal(stateVar + ".z3") + " ^ "
                    + Lcg.GetUintVal(stateVar + ".z4");
            }

        }

    }
}
