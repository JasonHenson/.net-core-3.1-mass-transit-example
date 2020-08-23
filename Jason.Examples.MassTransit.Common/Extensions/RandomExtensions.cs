using System;

namespace Jason.Examples.MassTransit.Common.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Get a random enum value for testing purposes.
        /// </summary>
        public static T GetRandomEnum<T>(this Random rnd) where T : Enum
        {
            return (T) (object) rnd.Next(0, Enum.GetNames(typeof(T)).Length);
        }
    }
}