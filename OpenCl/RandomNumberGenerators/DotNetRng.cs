using System;
namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public interface DotNetRng
    {
        uint GetUint();
        void Init(uint u);
        void Step();
    }

    public static class DotNetRngExtensions
    {
        public static float GetFloat(this DotNetRng rng)
        {
            return 2.32830629776081820e-10f * rng.GetUint();
        }

        public static float GetExponentialFloat(this DotNetRng rng)
        {
            float ret = (float)-Math.Log(1 - rng.GetFloat());
            return ret;
        }
    }
}
