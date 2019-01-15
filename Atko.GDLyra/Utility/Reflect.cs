using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Atko.GDLyra.Utility;
using Microsoft.CSharp;

using static System.Reflection.BindingFlags;
using static System.Reflection.MemberTypes;

namespace Atko.GDLyra.Utility
{
    internal static class Reflect
    {
        const string BackingFieldPrefix = "<";
        const string BackingFieldSuffix = ">k__BackingField";

        static ComputeCache<Tuple<Type, string, MemberTypes, BindingFlags>, MemberInfo> GetMemberCache { get; } =
            new ComputeCache<Tuple<Type, string, MemberTypes, BindingFlags>, MemberInfo>();

        static ComputeCache<PropertyInfo, FieldInfo> GetBackingFieldCache { get; } =
            new ComputeCache<PropertyInfo, FieldInfo>();

        static ComputeCache<Tuple<Type, Type>, bool> IsAssignableToGenericCache { get; } =
            new ComputeCache<Tuple<Type, Type>, bool>();

        public static void ClearCaches()
        {
            GetMemberCache.Clear();
            GetBackingFieldCache.Clear();
        }

        public static bool IsIntegerVariant(this Type type)
        {
            return
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong);
        }

        public static bool IsFloatVariant(this Type type)
        {
            return
                type == typeof(float) ||
                type == typeof(double);
        }

        public static IEnumerable<Type> Inheritance(this Type type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool IsDefined<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.IsDefined(typeof(TAttribute));
        }

        public static object DynamicGet(this object instance, string name)
        {
            DynamicGet(instance, name, out var value);
            return value;
        }

        public static bool DynamicGet(this object instance, string name, out object value)
        {
            var type = instance.GetType();
            var member = GetMember(type, name, Field | Property);
            if (member == null)
            {
                value = null;
                return false;
            }

            return DynamicGet(instance, member, out value);
        }

        public static object DynamicGet(this object instance, MemberInfo member)
        {
            DynamicGet(instance, member, out var value);
            return value;
        }

        public static bool DynamicGet(this object instance, MemberInfo member, out object value)
        {
            if (member is FieldInfo field)
            {
                value = field.GetValue(instance);
            }
            else if (member is PropertyInfo property && property.CanRead)
            {
                value = property.GetValue(instance);
            }
            else
            {
                value = null;
                return false;
            }

            return true;
        }

        public static bool DynamicSet(this object instance, string name, object value)
        {
            var type = instance.GetType();
            var member = GetMember(type, name, Field | Property);
            if (member == null)
            {
                return false;
            }

            return DynamicSet(instance, member, value);
        }

        public static bool DynamicSet(this object instance, MemberInfo member, object value)
        {
            if (member is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
            else if (member is PropertyInfo property)
            {
                if (property.CanWrite)
                {
                    property.SetValue(instance, value);
                }
                else
                {
                    GetBackingField(property).SetValue(instance, value);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool DynamicCall(this object instance, string name, params object[] args)
        {
            return DynamicCall(instance, name, out _, args);
        }

        public static bool DynamicCall(this object instance, string name, out object value, params object[] args)
        {
            var type = instance.GetType();
            if (GetMember(type, name) is MethodInfo method)
            {
                value = method.Invoke(instance, args);
                return true;
            }

            value = null;
            return false;
        }

        public static bool IsAssignableTo(this Type type, Type parent)
        {
            return parent.IsAssignableFrom(type);
        }

        public static bool IsAssignableToGeneric(this Type type, Type parent)
        {
            if (type.IsAssignableTo(parent))
            {
                return true;
            }

            return IsAssignableToGenericCache.Access(new Tuple<Type, Type>(type, parent), () =>
            {
                parent = parent.GetParameterlessType();

                foreach (var current in Inheritance(type))
                {
                    var generic = current.GetParameterlessType();
                    if (parent == generic || generic.ImplementsInterfaceGeneric(parent))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        public static Type GetParameterlessType(this Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition();
            }

            return type;
        }

        public static MemberInfo GetMember(Type type, string name, MemberTypes types = Field | Property | Method,
            BindingFlags flags = Instance | Public | NonPublic)
        {
            var key = new Tuple<Type, string, MemberTypes, BindingFlags>(type, name, types, flags);
            return GetMemberCache.Access(key, () =>
            {
                return type.GetMember(name, types, flags).FirstOrDefault();
            });
        }

        public static Type GetReturnType(this MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            if (member is MethodInfo method)
            {
                return method.ReturnType;
            }

            return null;
        }

        public static FieldInfo GetBackingField(PropertyInfo property)
        {
            return GetBackingFieldCache.Access(property, (input) =>
            {
                if (input.GetGetMethod(true).IsDefined(typeof(CompilerGeneratedAttribute)))
                {
                    var name = $"{BackingFieldPrefix}{input.Name}{BackingFieldSuffix}";
                    var type = input.DeclaringType;
                    if (type == null)
                    {
                        return null;
                    }

                    var field = type.GetField(name, Instance | NonPublic);
                    if (field == null)
                    {
                        return null;
                    }

                    if (field.IsDefined(typeof(CompilerGeneratedAttribute)))
                    {
                        return field;
                    }

                    return null;
                }

                return null;
            });
        }

        static bool ImplementsInterfaceGeneric(this Type type, Type implemented)
        {
            implemented = GetParameterlessType(implemented);
            foreach (var current in type.GetInterfaces())
            {
                if (current.GetParameterlessType() == implemented)
                {
                    return true;
                }
            }

            return false;
        }

    }

}