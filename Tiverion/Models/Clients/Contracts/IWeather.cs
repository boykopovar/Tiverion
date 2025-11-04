using System;
using Tiverion.Models.Entities.ServiceEntities.Weather;

namespace Tiverion.Models.Clients.Contracts
{
    public interface IWeather
    {
        /// <summary>Облачность</summary>
        Cloudiness Cloudiness { get; }

        /// <summary>Погодное состояние</summary>
        Condition Condition { get; }

        /// <summary>Время суток</summary>
        Daytime Daytime { get; }

        /// <summary>Фактическая температура в градусах Цельсия</summary>
        float TemperatureCels { get; }

        /// <summary>Ощущаемая температура в градусах Цельсия</summary>
        float FeelsLikeCels { get; }

        /// <summary>Влажность воздуха в процентах</summary>
        int Humidity { get; }

        /// <summary>Направление ветра</summary>
        WindDirection WindDirection { get; }

        /// <summary>Скорость ветра в м/с</summary>
        float WindSpeedMpS { get; }

        /// <summary>Давление в мм рт. ст.</summary>
        int PressureMmHg { get; }

        /// <summary>Информация о загрязнении воздуха</summary>
        PollutionInfo Pollution { get; }
    }
}