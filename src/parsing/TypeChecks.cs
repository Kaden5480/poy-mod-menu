using System;

namespace ModMenu.Parsing {
    /**
     * <summary>
     * A painful class with some code I really don't like,
     * but I couldn't think of anything better in the moment.
     * </summary>
     */
    internal static class TypeChecks {
        /**
         * <summary>
         * Checks if the provided type is
         * numeric.
         * </summary>
         */
        internal static bool IsNumeric(Type type) {
            return type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(char)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double);
        }

        /**
         * <summary>
         * Validates numeric types from strings.
         * </summary>
         * <param name="type">The type to validate for</param>
         * <param name="str">The string to validate</param>
         * <param name="result">The result from parsing</param>
         * <returns>Whether the string parsed successfully</returns>
         */
        internal static bool TryParse(Type type, string str, out object result) {
            bool valid = false;

            if (type == typeof(byte)) {
                byte res;
                valid = byte.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(sbyte)) {
                sbyte res;
                valid = sbyte.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(char)) {
                char res;
                valid = char.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(int)) {
                int res;
                valid = int.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(long)) {
                long res;
                valid = long.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(float)) {
                float res;
                valid = float.TryParse(str, out res);
                result = (object) res;
            }
            else if (type == typeof(double)) {
                double res;
                valid = double.TryParse(str, out res);
                result = (object) res;
            }
            else {
                result = null;
            }

            return valid;
        }
    }
}
