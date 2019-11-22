using SIF;

namespace Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var filename = "/Volumes/data/workspaces/Game/nxengine-evo/nxengine-evo/bin/data/sprites.sif";
            new SIFLoader(filename);
        }
    }
}