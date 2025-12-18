using System;
using System.Reflection;

using HarmonyLib;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A class for validating some information about
     * certain types.
     * </summary>
     */
    internal static class TypeChecks {
        /**
         * <summary>
         * Checks if the provided type is a whole number type.
         * </summary>
         * <param name="type">The type to check</param>
         * <returns>Whether the type is a whole number</returns>
         */
        internal static bool IsInteger(Type type) {
            return type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(char)
                || type == typeof(int)
                || type == typeof(long);
        }

        /**
         * <summary>
         * Checks if the provided type is numeric.
         * </summary>
         * <param name="type">The type to check</param>
         * <returns>Whether the type is numeric</returns>
         */
        internal static bool IsNumeric(Type type) {
            return IsInteger(type) == true
                || type == typeof(float)
                || type == typeof(double);
        }

        /**
         * <summary>
         * Parses numeric types from strings.
         * </summary>
         * <param name="type">The type to validate for</param>
         * <param name="str">The string to validate</param>
         * <param name="result">The result from parsing</param>
         * <returns>Whether the string parsed successfully</returns>
         */
        internal static bool TryParse(Type type, string str, out object result) {
            object[] args = new[] { str, null };
            result = null;

            // string types don't need parsing
            if (type == typeof(string)) {
                result = str;
                return true;
            }

            MethodInfo info = AccessTools.Method(
                type, "TryParse",
                new[] { typeof(string), type.MakeByRefType() }
            );

            if (info == null) {
                return false;
            }

            if ((bool) info.Invoke(null, args) == true) {
                result = args[1];
                return true;
            }

            return false;
        }

        /**
         * <summary>
         * Checks a value is within the provided limits
         * </summary>
         * <param name="value">The value to check</param>
         * <param name="min">The minimum value</param>
         * <param name="max">The maximum value</param>
         * <returns>Whether the value is within the provided limits</returns>
         */
        internal static bool InLimits(object value, object min, object max) {
            MethodInfo info = AccessTools.Method(
                value.GetType(), "CompareTo",
                new[] { value.GetType().MakeByRefType() }
            );

            if (info == null) {
                return false;
            }

            return ((int) info.Invoke(value, new[] { max })) <= 0
                && ((int) info.Invoke(min, new[] { value })) <= 0;
        }

        /**
         * <summary>
         * Returns a more user friendly string representation of a type.
         * </summary>
         * <param name="type">The type to get a string for</param>
         * <returns>The string representation</returns>
         */
        internal static string TypeToString(Type type) {
            if (IsInteger(type) == true) {
                return "Whole Number";
            }

            if (IsNumeric(type) == true) {
                return "Number";
            }

            if (type == typeof(string)) {
                return "Text";
            }

            return type.ToString();
        }
    }
}
