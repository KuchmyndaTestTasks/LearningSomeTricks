using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Xamarin.Forms.Maps;
using static System.Math;

namespace App.ReactiveUI.Xamarin.Maps.Services
{
    static class MapCalculator
    {
        internal enum DistanceUnit
        {
            Kilometer,
            Mile
        }

        public static MapSpan GetCenteredZoom(Position f, Position s)
        {
            var distance = CalculateDistance(f, s);
            var center = MidPoint(f, s);
            var distancia = new Distance(distance + 0.2);
            return MapSpan.FromCenterAndRadius(center, distancia);
        }

        private static double CalculateDistance(Position f, Position s)
        {
            var r = 6371e3; 
            var φ1 = DegToRad(f.Latitude);
            var φ2 = DegToRad(s.Latitude);
            var λ1 = DegToRad(f.Longitude);
            var λ2 = DegToRad(s.Longitude);

            var dφ = φ2 - φ1;
            var dλ = λ2 - λ1;
            var a = Sin(dφ / 2) * Sin(dφ / 2)
                + Cos(φ1) * Cos(φ2) * Sin(dλ / 2) * Sin(dλ / 2);

            var c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));

            return r * c;
        }

        public static Position MidPoint(Position f, Position s)
        {
            var φ1 = DegToRad(f.Latitude);
            var φ2 = DegToRad(s.Latitude);
            var λ1 = DegToRad(f.Longitude);
            var λ2 = DegToRad(s.Longitude);
            if (Abs(λ2 - λ1) > PI)
            {
                λ1 += 2 * PI; //anti-meridian
            }
            var φ3 = (φ1 + φ2) / 2;
            var quartOfPi = PI / 4;
            var f1 = Tan(quartOfPi + φ1 / 2);
            var f2 = Tan(quartOfPi + φ2 / 2);
            var f3 = Tan(quartOfPi + φ3 / 2);
            var λ3 = ((λ2 - λ1) * Log(f3) + λ1 * Log(f2) - λ2 * Log(f1)) / Log(f2 / f1);
            if (!double.IsInfinity(λ3))
            {
                λ3 = (λ1 + λ2) / 2;
            }
            return new Position(RadToDeg(φ3), (RadToDeg(λ3) + 540) % 360 - 180); //for normalising to -180..180
        }

        public static Position CenterOfPoints(params Position[] positions)
        {
            var lon = positions.Sum(x => x.Longitude);
            var lat = positions.Sum(x => x.Latitude);
            var size = positions.Length;
            return  new Position(lon/size, lat/size);
        }
        public static double CalculateDistance(Position from, Position to, DistanceUnit unit)
        {
            var theta = from.Longitude - to.Longitude;
            var dist = Sin(DegToRad(from.Latitude)) * Sin(DegToRad(to.Latitude)) +
                       Cos(DegToRad(from.Latitude)) * Cos(DegToRad(to.Latitude)) * Cos(DegToRad(theta));
            dist = Acos(dist);
            dist = RadToDeg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == DistanceUnit.Kilometer)
            {
                dist *= 1.609344;
            }
            else if (unit == DistanceUnit.Mile)
            {
                dist *= .8684;
            }
            return dist;
        }

        private static double DegToRad(double deg) => deg * PI / 180;
        private static double RadToDeg(double rad) => rad / PI * 180;
    }
}
