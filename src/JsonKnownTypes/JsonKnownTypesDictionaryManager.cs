﻿using System;
using System.Collections.Generic;
using System.Linq;
using JsonKnownTypes.Exceptions;

namespace JsonKnownTypes
{
    internal static class JsonKnownTypesDictionaryManager
    {
        public static void AddJsonKnown<T>(this Dictionary<Type, string> dictionary)
        {
            var attrs = AttributeManager.GetJsonKnownAttributes(typeof(T));
            foreach (var attr in attrs)
            {
                var discriminator = attr.Discriminator ?? attr.Type.Name;
                dictionary.TryAdd(attr.Type, discriminator);
            }
        }

        public static void AddJsonKnownThis(this Dictionary<Type, string> dictionary, Type[] inherited)
        {
            foreach (var type in inherited)
            {
                var attr = AttributeManager.GetJsonKnownThisAttribute(type);
                if (attr != null)
                {
                    var discriminator = attr.Discriminator ?? type.Name;
                    dictionary.TryAdd(type, discriminator);
                }
            }
        }

        public static void AddAutoGenerated(this Dictionary<Type, string> dictionary, Type[] inherited)
        {
            foreach (var type in inherited.Except(dictionary.Keys))
            {
                dictionary.TryAdd(type, type.Name);
            }
        }

        public static void TryAdd(this Dictionary<Type, string> dictionary, Type key, string discriminator)
        {
            if (dictionary.ContainsKey(key))
                throw new JsonKnownTypesException($"Dictionary already contains the key {key.Name}");//todo

            if (dictionary.ContainsValue(discriminator))
                throw new JsonKnownTypesException($"Dictionary already contains the value {discriminator}");//todo

            dictionary.Add(key, discriminator);
        }

        public static Dictionary<string, Type> Revert(this Dictionary<Type, string> dictionary)
        {
            try
            {
                return dictionary.ToDictionary(x => x.Value, x => x.Key);
            }
            catch (ArgumentException e)
            {
                throw new JsonKnownTypesException("Dictionary has a repeated value", e);
            }
        }
    }
}
