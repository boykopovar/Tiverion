using Tiverion.Data.Constants;

namespace Tiverion.Models.Entities.ServiceEntities
{
    public class MapPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

        public MapPoint(double lat, double lon)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentOutOfRangeException(nameof(lat), "Lat должен быть между -90 и 90");
            if (lon < -180 || lon > 180)
                throw new ArgumentOutOfRangeException(nameof(lon), "Lon должен быть между -180 и 180");

            Lat = lat;
            Lon = lon;
        }

        private MapPoint() {}

        public override string ToString() => $"{Lat.ToString(PlatformSettings.DefaultCulture)},{Lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        

        public const double EarthRadius = 6371008.7714;
        
        
        /// <summary>
        /// Вычисляет центральную точку между двумя точками.
        /// </summary>
        /// <param name="point1">Первая точка</param>
        /// <param name="point2">Вторая точка</param>
        /// <returns>Новая MapPoint, находящаяся в центре между двумя точками</returns>
        public static MapPoint GetCenter(MapPoint point1, MapPoint point2)
        {
            // if (point1 == null) throw new ArgumentNullException(nameof(point1));
            // if (point2 == null) throw new ArgumentNullException(nameof(point2));

            double centerLat = (point1.Lat + point2.Lat) / 2;
            double centerLon = (point1.Lon + point2.Lon) / 2;

            return new MapPoint(centerLat, centerLon);
        }

        /// <summary>
        /// Вычисляет расстояние в метрах между двумя точками на Земле
        /// с использованием формулы Haversine.
        /// </summary>
        /// <param name="point1">Первая точка</param>
        /// <param name="point2">Вторая точка</param>
        /// <returns>Расстояние в метрах</returns>
        public static double GetDistanceMeters(MapPoint point1, MapPoint point2)
        {
            if (point1 == null) throw new ArgumentNullException(nameof(point1));
            if (point2 == null) throw new ArgumentNullException(nameof(point2));
            
            double lat1Rad = DegreesToRadians(point1.Lat);
            double lat2Rad = DegreesToRadians(point2.Lat);
            double deltaLat = DegreesToRadians(point2.Lat - point1.Lat);
            double deltaLon = DegreesToRadians(point2.Lon - point1.Lon);

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c;
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
        
        private static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;

        
        /// <summary>
        /// Возвращает квадратную область (bounding box) вокруг центральной точки
        /// по заданному радиусу вписанной окружности (в метрах).
        /// Юго-западный (SW) и северо-восточный (NE) углы.
        /// </summary>
        /// <param name="center">Центральная точка</param>
        /// <param name="radiusMeters">Радиус вписанной окружности в метрах</param>
        /// <returns>Tuple: юго-западный и северо-восточный угол</returns>
        public static (MapPoint SouthWest, MapPoint NorthEast) GetBoundingBox(MapPoint center, double radiusMeters)
        {
            if (center == null) throw new ArgumentNullException(nameof(center));
            if (radiusMeters < 0) throw new ArgumentOutOfRangeException(nameof(radiusMeters));

            double deltaLat = RadiansToDegrees(radiusMeters / EarthRadius);
            double deltaLon = RadiansToDegrees(radiusMeters / (EarthRadius * Math.Cos(DegreesToRadians(center.Lat))));

            MapPoint southWest = new MapPoint(center.Lat - deltaLat, center.Lon - deltaLon);
            MapPoint northEast = new MapPoint(center.Lat + deltaLat, center.Lon + deltaLon);

            return (southWest, northEast);
        }

        public static (MapPoint Center, double RadiusMeters) GetCenterAndRadius(MapPoint southWest, MapPoint northEast)
        {
            if (southWest == null) throw new ArgumentNullException(nameof(southWest));
            if (northEast == null) throw new ArgumentNullException(nameof(northEast));

            double centerLat = (southWest.Lat + northEast.Lat) / 2.0;
            double centerLon = (southWest.Lon + northEast.Lon) / 2.0;

            double deltaLat = Math.Abs(northEast.Lat - centerLat);
            double deltaLon = Math.Abs(northEast.Lon - centerLon);

            double radiusLat = DegreesToRadians(deltaLat) * EarthRadius;
            double radiusLon = DegreesToRadians(deltaLon) * EarthRadius * Math.Cos(DegreesToRadians(centerLat));

            double radiusMeters = Math.Sqrt(radiusLat * radiusLat + radiusLon * radiusLon);

            return (new MapPoint(centerLat, centerLon), radiusMeters);
        }


        /// <summary>
        /// Вычисляет площадь прямоугольной области в квадратных метрах между двумя точками
        /// (юго-западной и северо-восточной).
        /// </summary>
        /// <param name="point1">Первая точка (юго-западная)</param>
        /// <param name="point2">Вторая точка (северо-восточная)</param>
        /// <returns>Площадь в квадратных метрах</returns>
        public static double GetAreaSqMetres(MapPoint point1, MapPoint point2)
        {
            if (point1 == null) throw new ArgumentNullException(nameof(point1));
            if (point2 == null) throw new ArgumentNullException(nameof(point2));

            double lat1 = DegreesToRadians(point1.Lat);
            double lat2 = DegreesToRadians(point2.Lat);
            double lon1 = DegreesToRadians(point1.Lon);
            double lon2 = DegreesToRadians(point2.Lon);

            double deltaLat = Math.Abs(lat2 - lat1);
            double deltaLon = Math.Abs(lon2 - lon1);
            double meanLat = (lat1 + lat2) / 2.0;

            double height = EarthRadius * deltaLat;
            double width = EarthRadius * deltaLon * Math.Cos(meanLat);

            return height * width;
        }

    }
}
