using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Domain.Modules.ErrorOrigins;
using Coderr.Server.ReportAnalyzer.ErrorOrigins.Handlers;
using Griffin.ApplicationServices;
using Newtonsoft.Json.Linq;

namespace Coderr.Server.ReportAnalyzer.ErrorOrigins
{
    [ContainerService(RegisterAsSelf = true)]
    public class LookupInboundOrigins : IBackgroundJobAsync
    {
        private readonly IErrorOriginRepository _errorOriginRepository;
        private readonly IConfiguration<OriginsConfiguration> _originConfiguration;

        public LookupInboundOrigins(IErrorOriginRepository errorOriginRepository, IConfiguration<OriginsConfiguration> originConfiguration)
        {
            _errorOriginRepository = errorOriginRepository;
            _originConfiguration = originConfiguration;
        }

        public async Task ExecuteAsync()
        {
            var toLookup = await _errorOriginRepository.GetPendingOrigins();
            foreach (var origin in toLookup)
            {
                if (origin.Longitude > 180)
                {
                    //await LookupIpAddress(origin);
                    await CommercialLookup(origin);
                    await _errorOriginRepository.Update(origin);
                    continue;
                }

                if (!string.IsNullOrEmpty(_originConfiguration.Value.LocationIqApiKey))
                {
                    await LookupCoordinatesUsingLocationIq(_originConfiguration.Value.LocationIqApiKey, origin);
                }
                else if (!string.IsNullOrEmpty(_originConfiguration.Value.MapQuestApiKey))
                {
                    await LookupCoordinatesUsingMapQuest(_originConfiguration.Value.MapQuestApiKey, origin);
                }

                await _errorOriginRepository.Update(origin);
            }
        }

        private async Task LookupIpAddress(ErrorOrigin origin)
        {
            if (string.IsNullOrEmpty(_originConfiguration.Value.ApiKey))
                return;

            var url = $"http://api.ipstack.com/{origin.IpAddress}?access_key={_originConfiguration.Value.ApiKey}";
            var request = WebRequest.CreateHttp(url);
            var json = "";
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                /*
                    {
                       "ip":"94.254.21.175",
                       "country_code":"SE",
                       "country_name":"Sweden",
                       "region_code":"10",
                       "region_name":"Dalarnas Lan",
                       "city":"Falun",
                       "zipcode":"",
                       "latitude":60.6,
                       "longitude":15.6333,
                       "metro_code":"",
                       "areacode":""
                    }
                 */

                // Key is only included in error messages.
                if (jsonObj.ContainsKey("success"))
                {
                    return;
                }

                var lat = double.Parse(jsonObj["latitude"].Value<string>(), CultureInfo.InvariantCulture);
                var lon = double.Parse(jsonObj["longitude"].Value<string>(), CultureInfo.InvariantCulture);
                origin.Longitude = lon;
                origin.Latitude = lat;
                origin.City = jsonObj["city"].ToString();
                origin.CountryCode = jsonObj["country_code"].ToString();
                origin.CountryName = jsonObj["country_name"].ToString();
                origin.RegionCode = jsonObj["region_code"].ToString();
                origin.RegionName = jsonObj["region_name"].ToString();
                origin.ZipCode = jsonObj["zip"].ToString();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to call lookupService or parse the JSON: {json}.", exception);
            }
        }

        private async Task LookupCoordinatesUsingMapQuest(string mapQuestKey, ErrorOrigin origin)
        {
            #region JSON example
            /*
                {
                  "info": {
                    "statuscode": 0,
                    "copyright": {
                      "text": "© 2018 MapQuest, Inc.",
                      "imageUrl": "http://api.mqcdn.com/res/mqlogo.gif",
                      "imageAltText": "© 2018 MapQuest, Inc."
                    },
                    "messages": []
                  },
                  "options": {
                    "maxResults": 1,
                    "thumbMaps": true,
                    "ignoreLatLngInput": false
                  },
                  "results": [
                    {
                      "providedLocation": {
                        "latLng": {
                          "lat": 30.333472,
                          "lng": -81.470448
                        }
                      },
                      "locations": [
                        {
                          "street": "12714 Ashley Melisse Blvd",
                          "adminArea6": "",
                          "adminArea6Type": "Neighborhood",
                          "adminArea5": "Jacksonville",
                          "adminArea5Type": "City",
                          "adminArea4": "Duval",
                          "adminArea4Type": "County",
                          "adminArea3": "FL",
                          "adminArea3Type": "State",
                          "adminArea1": "US",
                          "adminArea1Type": "Country",
                          "postalCode": "32225",
                          "geocodeQualityCode": "L1AAA",
                          "geocodeQuality": "ADDRESS",
                          "dragPoint": false,
                          "sideOfStreet": "R",
                          "linkId": "0",
                          "unknownInput": "",
                          "type": "s",
                          "latLng": {
                            "lat": 30.33472,
                            "lng": -81.470448
                          },
                          "displayLatLng": {
                            "lat": 30.333472,
                            "lng": -81.470448
                          },
                          "mapUrl": "http://open.mapquestapi.com/staticmap/v4/getmap?key=KEY&type=map&size=225,160&pois=purple-1,30.3334721,-81.4704483,0,0,|&center=30.3334721,-81.4704483&zoom=15&rand=-553163060",
                          "nearestIntersection": {
                            "streetDisplayName": "Posey Cir",
                            "distanceMeters": "851755.1608527573",
                            "latLng": {
                              "longitude": -87.523761,
                              "latitude": 35.013434
                            },
                            "label": "Danley Rd & Posey Cir"
                          },
                          "roadMetadata": {
                            "speedLimitUnits": "mph",
                            "tollRoad": null,
                            "speedLimit": 40
                          }
                        }
                      ]
                    }
                  ]
                }
             */
            #endregion

            var url =
                $"http://open.mapquestapi.com/geocoding/v1/reverse?key={mapQuestKey}&location={origin.Latitude.ToString(CultureInfo.InvariantCulture)},{origin.Longitude.ToString(CultureInfo.InvariantCulture)}&includeRoadMetadata=true&includeNearestIntersection=true";
            var request = WebRequest.CreateHttp(url);
            var json = "";
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                var array = (JArray)jsonObj["results"][0]["locations"];
                if (array.Count == 0)
                    return;

                var firstLocation = jsonObj["results"][0]["locations"][0];
                origin.ZipCode = firstLocation["postalCode"].Value<string>();

                /*  "street": "12714 Ashley Melisse Blvd",
                          "adminArea6": "",
                          "adminArea6Type": "Neighborhood",
                          "adminArea5": "Jacksonville",
                          "adminArea5Type": "City",
                          "adminArea4": "Duval",
                          "adminArea4Type": "County",
                          "adminArea3": "FL",
                          "adminArea3Type": "State",
                          "adminArea1": "US",
                          "adminArea1Type": "Country",
                          "postalCode": "32225",*/

                for (var i = 1; i <= 6; i++)
                {
                    var token = firstLocation[$"adminArea{i}"];
                    var value = token?.Value<string>();
                    if (value == null)
                        continue;

                    switch (firstLocation[$"adminArea{i}Type"].Value<string>())
                    {
                        case "Country":
                            origin.CountryCode = value;
                            break;
                        case "State":
                            origin.RegionName = value;
                            break;
                        case "County":
                            break;
                        case "City":
                            origin.City = value;
                            break;
                    }
                }

                //origin.CountryName = firstLocation["country_name"].ToString();
                //origin.RegionCode = firstLocation["region_code"].ToString();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to call lookupService or parse the JSON: {json}.", exception);
            }
        }

        private async Task LookupCoordinatesUsingLocationIq(string apiKey, ErrorOrigin origin)
        {
            /*
                    {
                        "place_id": "26693344",
                        "licence": "© LocationIQ.com CC BY 4.0, Data © OpenStreetMap contributors, ODbL 1.0",
                        "osm_type": "node",
                        "osm_id": "2525193585",
                        "lat": "-37.870662",
                        "lon": "144.9803321",
                        "display_name": "Imbiss 25, Blessington Street, St Kilda, City of Port Phillip, Greater Melbourne, Victoria, 3182, Australia",
                        "address": {
                            "cafe": "Imbiss 25",
                            "road": "Blessington Street",
                            "suburb": "St Kilda",
                            "county": "City of Port Phillip",
                            "region": "Greater Melbourne",
                            "state": "Victoria",
                            "postcode": "3182",
                            "country": "Australia",
                            "country_code": "au"
                        },
                        "boundingbox": [
                            "-37.870762",
                            "-37.870562",
                            "144.9802321",
                            "144.9804321"
                        ]
                    }
             */
            var url =
                $"https://us1.locationiq.com/v1/reverse.php?key={apiKey}&lat={origin.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={origin.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json";

            var json = "";
            try
            {
                var request = WebRequest.CreateHttp(url);
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                var address = jsonObj["address"];
                if (address == null)
                {
                    Err.ReportLogicError("Failed to lookup long/lat.", origin, "OriginLookup2");
                    return;
                }

                /*  "address": {
                        "cafe": "Imbiss 25",
                        "road": "Blessington Street",
                        "suburb": "St Kilda",
                        "county": "City of Port Phillip",
                        "region": "Greater Melbourne",
                        "state": "Victoria",
                        "postcode": "3182",
                        "country": "Australia",
                        "country_code": "au"
                    }*/
                origin.City = address["city"]?.Value<string>();
                origin.CountryCode = address["country_code"]?.Value<string>();
                origin.CountryName = address["country"]?.Value<string>();
                origin.RegionCode = address["region_code"]?.Value<string>();
                origin.RegionName = address["state"]?.Value<string>();
                origin.ZipCode = address["postcode"]?.Value<string>();

            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to call lookupService or parse the JSON: {json}.", exception);
            }


        }

        private async Task CommercialLookup(ErrorOrigin origin)
        {
            //fe80::1855:17f1:43ab:cc48%5 TODO: What do the %5 mean?
            var pos = origin.IpAddress.IndexOf('%');
            var ip = pos == -1 ? origin.IpAddress : origin.IpAddress.Substring(0, pos);

            var request = WebRequest.CreateHttp("https://timezoneapi.io/api/ip/?token=aAakJWmQjzgKudYTMmiV&ip=" + ip);
            var json = "";
            try
            {
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
                var jsonObj = JObject.Parse(json);

                /** Not found:
                 * {"meta":{"code":"200","execution_time":"0.003025 seconds"},"data":{"ip":"151.248.19.34","city":"","postal":"","state":"","state_code":"","country":"","country_code":"","location":"","timezone":null,"datetime":null}}
                 */

                //data { "location": "51.5062,-0.0196 }
                var location = (string)jsonObj["data"]["location"];
                if (string.IsNullOrEmpty(location))
                {
                    Err.ReportLogicError("Failed to lookup long/lat.", origin, "OriginLookup");
                    return;
                }
                    

                var parts = location.Split(',');
                var lat = double.Parse(parts[0], CultureInfo.InvariantCulture);
                var lon = double.Parse(parts[1], CultureInfo.InvariantCulture);
                var data = jsonObj["data"];
                origin.City = data["city"].ToString();
                origin.CountryCode = data["country_code"].ToString();
                origin.Latitude = lat;
                origin.Longitude = lon;
                origin.CountryName = data["country"].ToString();
                origin.RegionCode = data["state_code"].ToString();
                origin.RegionName = data["state"].ToString();
                origin.ZipCode = data["postal"].ToString();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to call lookupService or parse the JSON: {json}.", exception);
            }
        }
    }
}
