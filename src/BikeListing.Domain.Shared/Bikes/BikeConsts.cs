namespace BikeListing.Bikes
{
    public static class BikeConsts
    {
        private const string DefaultSorting = "{0}Model asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Bike." : string.Empty);
        }

        public const int ModelMinLength = 1;
        public const int ModelMaxLength = 100;
    }
}