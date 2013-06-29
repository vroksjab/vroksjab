using System.Collections.Generic;

namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public static class Generators
    {
        public sealed class GeneratorEntry
        {
            public readonly string Name;
            public readonly RandomGenerator Generator;

            public GeneratorEntry(string name, RandomGenerator generator)
            {
                this.Name = name;
                this.Generator = generator;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }

        public static readonly RandomGenerator Tau1;
        public static readonly RandomGenerator Tau2;
        public static readonly RandomGenerator Tau3;
        public static readonly RandomGenerator Lcg1;

        public static readonly RandomGenerator Zero = new ZeroRng();
        public static readonly RandomGenerator One = new OneRng();
        public static readonly RandomGenerator Max = new MaxRng();

        public static readonly List<GeneratorEntry> AllGenerators = new List<GeneratorEntry>();

        public static readonly RandomGenerator HybridTau;

        static Generators()
        {
            // The magic constants for initXorValues are picked by my fingers and keyboard.
            Taus tau1 = new Taus(13, 19, 12, 4294967294u, 256, 0xA2F47E12u, 0);  // p1=2^31-1 
            Taus tau2 = new Taus(2, 25, 4, 4294967288u, 512, 0xD3316298u, 29);   // p2=2^30-1  
            Taus tau3 = new Taus(3, 11, 17, 4294967280u, 1024, 0x1D454AF1u, 30);   // p3=2^28-1  
            Lcg lcg1 = new Lcg(1664525, 1013904223u, 0, 0xE0F39837u, 31);        // p4=2^32  
            HybridTau = new HybridTau(tau1, tau2, tau3, lcg1);
            AllGenerators.Add(new GeneratorEntry("HybridTau", HybridTau));

            Tau1 = tau1;
            AllGenerators.Add(new GeneratorEntry("Tau1", Tau1));
            Tau2 = tau2;
            AllGenerators.Add(new GeneratorEntry("Tau2", Tau2));
            Tau3 = tau3;
            AllGenerators.Add(new GeneratorEntry("Tau3", Tau3));
            Lcg1 = lcg1;
            AllGenerators.Add(new GeneratorEntry("Lcg1", Lcg1));

            AllGenerators.Add(new GeneratorEntry("Zero", Zero));
            AllGenerators.Add(new GeneratorEntry("One", One));
            AllGenerators.Add(new GeneratorEntry("Max", Max));

        }
    }
}
