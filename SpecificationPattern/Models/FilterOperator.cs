namespace SpecificationPattern
{
    /*--- SPECIFIACTION PATTERN ---*/

    public static class FilterOperator
    {
        public const string Equal = "eq"; // ✅
        public const string NotEqual = "neq"; // ✅
        public const string IsNull = "isnull"; // ✅
        public const string IsNotNull = "isnotnull"; // ✅
        public const string LowerThan = "lt"; // ✅
        public const string LowerThanOrEqual = "lte"; // ✅
        public const string GreaterThan = "gt"; // ✅
        public const string GreaterThanOrEqual = "gte"; // ✅
        public const string StartsWith = "startswith"; // ✅
        public const string EndsWith = "endswith"; // ✅
        public const string Contains = "contains"; // ✅
        public const string DoesNotContains = "doesnotcontain";
        public const string ContainsList = "containslist";
        public const string DoesNotContainsList = "doesnotcontainlist";
        public const string IsEmpty = "isempty"; // ✅
        public const string IsNotEmpty = "isnotempty"; // ✅
        public const string InDay = "inday";
        public const string NotInDay = "ninday";
        public const string GreaterThanDay = "gtinday";
        public const string GreaterThanOrEqualDay = "gteinday";
        public const string LowerThanDay = "ltinday";
        public const string LowerThanOrEqualDay = "lteinday";
    }

    public static class FilterLogic
    {
        public const string And = "and";
        public const string Or = "or";
    }
}