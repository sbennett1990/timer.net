
namespace Timer.Utils {
    public static class Extensions {
        public static int? ToInt(this string s) {
            if (IsEmpty(s)) {
                return null;
            }
            else {
                if (int.TryParse(s, out int result)) {
                    return result;
                }
                else {
                    return null;
                }
            }
        }

        public static bool IsEmpty(this string s) {
            if (string.IsNullOrWhiteSpace(s)) {
                return true;
            }
            else {
                return false;
            }
        }

        public static bool IsNotEmpty(this string s) {
            return !IsEmpty(s);
        }
    }
}
