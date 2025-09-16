namespace Client.IpGeoLocation
{
    public static class GeoInformationFactory
    {
        private static readonly GeoInformationRetriever Retriever = new GeoInformationRetriever();
        private static GeoInformation _geoInformation;
        private static DateTime _lastSuccessfulLocation = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const int MINIMUM_VALID_TIME = 60 * 12;

        public static GeoInformation GetGeoInformation()
        {
            var passedTime = new TimeSpan(DateTime.UtcNow.Ticks - _lastSuccessfulLocation.Ticks);

            if (_geoInformation == null || passedTime.TotalMinutes > MINIMUM_VALID_TIME)
            {
                _geoInformation = Retriever.Retrieve();
                _lastSuccessfulLocation = DateTime.UtcNow;
            }

            return _geoInformation;
        }
    }
}
