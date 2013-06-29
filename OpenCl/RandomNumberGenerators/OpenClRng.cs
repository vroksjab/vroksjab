namespace Vroksjab.OpenCl.RandomNumberGenerators
{
    public interface OpenClRng
    {
        string TypeName { get; }
        string DefineStateAndFunctions();
        string DeclareState(string stateVar);
        string InitState(string stateVar, string seedVar);
        string StupidInitState(string stateVar, string seedVar);
        string StepState(string stateVar);

        /// <summary>
        /// This returns an _expression_ which evaulates to un unsigned value.
        /// </summary>
        string GetUintVal(string stateVar);
    }

    public static class OpelClRngExtensions
    {
        public static string InitState(this OpenClRng rng, string stateVar, string seedVar, bool beStupid)
        {
            if (beStupid)
            {
                return rng.StupidInitState(stateVar, seedVar);
            }
            else
            {
                return rng.InitState(stateVar, seedVar);
            }
        }
    }
}
