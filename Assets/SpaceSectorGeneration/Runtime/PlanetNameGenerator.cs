using System;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public static class PlanetNameGenerator
    {
        public static string Generate(int seed)
        {
            System.Random random = new System.Random(seed);
            char suffix = (char)('b' + random.Next(0, 7));

            switch (random.Next(0, 10))
            {
                case 0:
                    return $"Kepler-{random.Next(1, 6001)}{suffix}";

                case 1:
                    return $"TOI-{random.Next(100, 10000)} {suffix}";

                case 2:
                    return $"WASP-{random.Next(1, 301)}{suffix}";

                case 3:
                    return $"HAT-P-{random.Next(1, 101)}{suffix}";

                case 4:
                    return $"HD {random.Next(1000, 999999)} {suffix}";

                case 5:
                    return $"GJ {random.Next(100, 2001)} {suffix}";

                case 6:
                    return $"K2-{random.Next(1, 501)}{suffix}";

                case 7:
                    return $"CoRoT-{random.Next(1, 51)}{suffix}";

                case 8:
                    return $"HIP {random.Next(1000, 99999)} {suffix}";

                default:
                    return $"TIC {random.Next(100000, 999999999)} {suffix}";
            }
        }
    }
}
