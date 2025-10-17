#nullable enable
using System;
using System.Collections.Generic;

namespace VHDPV2.Core
{
    public static class GameServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            Services[typeof(T)] = service;
        }

        public static void Unregister<T>() where T : class
        {
            Services.Remove(typeof(T));
        }

        public static T Require<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out object? service) && service is T typed)
            {
                return typed;
            }

            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }

        public static bool TryGet<T>(out T? service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out object? obj) && obj is T typed)
            {
                service = typed;
                return true;
            }

            service = null;
            return false;
        }
    }
}
