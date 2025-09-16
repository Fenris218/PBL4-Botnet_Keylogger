using Client.Helper;
using Common.Helper;
using System.Globalization;
using System.Net;

namespace Client.IpGeoLocation
{
    public class GeoInformationRetriever
    {
        public GeoInformation Retrieve()
        {
            var geo = TryRetrieveOnline() ?? TryRetrieveLocally();

            if (string.IsNullOrEmpty(geo.IpAddress))
                geo.IpAddress = TryGetWanIp();

            geo.IpAddress = (string.IsNullOrEmpty(geo.IpAddress)) ? "Unknown" : geo.IpAddress;
            geo.Country = (string.IsNullOrEmpty(geo.Country)) ? "Unknown" : geo.Country;
            geo.CountryCode = (string.IsNullOrEmpty(geo.CountryCode)) ? "-" : geo.CountryCode;
            geo.Timezone = (string.IsNullOrEmpty(geo.Timezone)) ? "Unknown" : geo.Timezone;
            geo.Asn = (string.IsNullOrEmpty(geo.Asn)) ? "Unknown" : geo.Asn;
            geo.Isp = (string.IsNullOrEmpty(geo.Isp)) ? "Unknown" : geo.Isp;

            return geo;
        }

        private GeoInformation TryRetrieveOnline()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ipwho.is/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0";
                request.Proxy = null;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        var geoInfo = JsonHelper.Deserialize<GeoResponse>(dataStream);

                        GeoInformation g = new GeoInformation
                        {
                            IpAddress = geoInfo.Ip,
                            Country = geoInfo.Country,
                            CountryCode = geoInfo.CountryCode,
                            Timezone = geoInfo.Timezone.UTC,
                            Asn = geoInfo.Connection.ASN.ToString(),
                            Isp = geoInfo.Connection.ISP
                        };

                        return g;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private GeoInformation TryRetrieveLocally()
        {
            try
            {
                GeoInformation g = new GeoInformation();

                var cultureInfo = CultureInfo.CurrentUICulture;
                var region = new RegionInfo(cultureInfo.LCID);

                g.Country = region.DisplayName;
                g.CountryCode = region.TwoLetterISORegionName;
                g.Timezone = DateTimeHelper.GetLocalTimeZone();

                return g;
            }
            catch
            {
                return null;
            }
        }

        private string TryGetWanIp()
        {
            string wanIp = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0";
                request.Proxy = null;
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            wanIp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
            }

            return wanIp;
        }
    }
}
