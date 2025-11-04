using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tiverion.Models.Entities.ServiceEntities.Weather
{
    public enum Sport
    {
        [EnumMember(Value = "RUN")] Run,
        [EnumMember(Value = "SKI")] Ski
    }

    public enum Cloudiness
    {
        [EnumMember(Value = "CLEAR")]
        [Display(Name = "Ясно")]
        Clear,

        [EnumMember(Value = "PARTLY")]
        [Display(Name = "Переменная облачность")]
        Partly,

        [EnumMember(Value = "SIGNIFICANT")]
        [Display(Name = "Значительная облачность")]
        Significant,

        [EnumMember(Value = "CLOUDY")]
        [Display(Name = "Облачно")]
        Cloudy,

        [EnumMember(Value = "OVERCAST")]
        [Display(Name = "Пасмурно")]
        Overcast
    }


    public enum Condition
    {
        [EnumMember(Value = "CLEAR")]
        [Display(Name = "Ясно")]
        Clear,

        [EnumMember(Value = "PARTLY_CLOUDY")]
        [Display(Name = "Переменная облачность")]
        PartlyCloudy,

        [EnumMember(Value = "CLOUDY")]
        [Display(Name = "Облачно")]
        Cloudy,

        [EnumMember(Value = "OVERCAST")]
        [Display(Name = "Пасмурно")]
        Overcast,

        [EnumMember(Value = "LIGHT_RAIN")]
        [Display(Name = "Небольшой дождь")]
        LightRain,

        [EnumMember(Value = "RAIN")]
        [Display(Name = "Дождь")]
        Rain,

        [EnumMember(Value = "HEAVY_RAIN")]
        [Display(Name = "Сильный дождь")]
        HeavyRain,

        [EnumMember(Value = "LIGHT_SNOW")]
        [Display(Name = "Небольшой снег")]
        LightSnow,

        [EnumMember(Value = "SNOW")]
        [Display(Name = "Снег")]
        Snow,

        [EnumMember(Value = "SLEET")]
        [Display(Name = "Снег с дождём")]
        Sleet,

        [EnumMember(Value = "THUNDERSTORM")]
        [Display(Name = "Гроза")]
        Thunderstorm,

        [EnumMember(Value = "THUNDERSTORM_WITH_RAIN")]
        [Display(Name = "Гроза с дождём")]
        ThunderstormWithRain
        
        
    }

    public enum Daytime
    {
        [EnumMember(Value = "DAY")] Day,
        [EnumMember(Value = "NIGHT")] Night
    }

    public enum PrecipitationStrength
    {
        [EnumMember(Value = "ZERO")] Zero,
        [EnumMember(Value = "WEAK")] Weak,
        [EnumMember(Value = "AVERAGE")] Average,
        [EnumMember(Value = "STRONG")] Strong
    }

    public enum PrecipitationType
    {
        [EnumMember(Value = "NO_TYPE")] NoType,
        [EnumMember(Value = "RAIN")] Rain,
        [EnumMember(Value = "SLEET")] Sleet,
        [EnumMember(Value = "SNOW")] Snow
    }

    public enum Season
    {
        [EnumMember(Value = "WINTER")] Winter,
        [EnumMember(Value = "SPRING")] Spring,
        [EnumMember(Value = "SUMMER")] Summer,
        [EnumMember(Value = "AUTUMN")] Autumn
    }

    public enum WindDirection
    {
        [EnumMember(Value = "CALM")]
        [Display(Name = "Штиль")]
        Calm,

        [EnumMember(Value = "NORTH")]
        [Display(Name = "Северный")]
        North,

        [EnumMember(Value = "NORTH_EAST")]
        [Display(Name = "Северо-восточный")]
        NorthEast,

        [EnumMember(Value = "EAST")]
        [Display(Name = "Восточный")]
        East,

        [EnumMember(Value = "SOUTH_EAST")]
        [Display(Name = "Юго-восточный")]
        SouthEast,

        [EnumMember(Value = "SOUTH")]
        [Display(Name = "Южный")]
        South,

        [EnumMember(Value = "SOUTH_WEST")]
        [Display(Name = "Юго-западный")]
        SouthWest,

        [EnumMember(Value = "WEST")]
        [Display(Name = "Западный")]
        West,

        [EnumMember(Value = "NORTH_WEST")]
        [Display(Name = "Северо-западный")]
        NorthWest
    }

    public enum PollutionDominant
    {
        [EnumMember(Value = "PM2p5")] Pm2p5,
        [EnumMember(Value = "PM10")] Pm10,
        [EnumMember(Value = "O3")] O3,
        [EnumMember(Value = "NO2")] No2,
        [EnumMember(Value = "SO2")] So2,
        [EnumMember(Value = "CO")] Co
    }

    public enum RunCondition
    {
        [EnumMember(Value = "WELL")] Well,
        [EnumMember(Value = "DISCOMFORT")] Discomfort,
        [EnumMember(Value = "UNDESIRABLE")] Undesirable
    }

    public enum Icon
    {
        [EnumMember(Value = "bkn_-ra_d")] BknMinusRaD,
        [EnumMember(Value = "bkn_-ra_n")] BknMinusRaN,
        [EnumMember(Value = "bkn_d")] BknD,
        [EnumMember(Value = "bkn_n")] BknN,
        [EnumMember(Value = "bkn_ra_n")] BknRaN,
        [EnumMember(Value = "bkn_ra_d")] BknRaD,
        [EnumMember(Value = "ovc")] Ovc,
        [EnumMember(Value = "ovc_-ra")] OvcMinusRa,
        [EnumMember(Value = "ovc_-sn")] OvcMinusSn,
        [EnumMember(Value = "ovc_ra")] OvcRa,
        [EnumMember(Value = "ovc_ra_sn")] OvcRaSn,
        [EnumMember(Value = "ovc_ts")] OvcTs,
        [EnumMember(Value = "ovc_ts_ra")] OvcTsRa,
        [EnumMember(Value = "skc_d")] SkcD,
        [EnumMember(Value = "skc_n")] SkcN
    }

    public enum RoadCondition
    {
        [EnumMember(Value = "DRY")] Dry,
        [EnumMember(Value = "LOOSE_SNOW")] LooseSnow,
        [EnumMember(Value = "WET")] Wet,
        [EnumMember(Value = "ROLLED_SNOW")] RolledSnow,
        [EnumMember(Value = "BLACK_ICE")] BlackIce
    }

    public enum WarningCode
    {
        [EnumMember(Value = "NowGoodWeatherForSport")] NowGoodWeatherForSport,
        [EnumMember(Value = "NowBadWeatherForSport")] NowBadWeatherForSport,
        [EnumMember(Value = "NowDiscomfortWeatherForSport")] NowDiscomfortWeatherForSport,
        [EnumMember(Value = "Nowcast")] Nowcast,
        [EnumMember(Value = "GroupedNextWeek")] GroupedNextWeek,
        [EnumMember(Value = "GroupedWeekend")] GroupedWeekend,
        [EnumMember(Value = "GettingColderNextWeek")] GettingColderNextWeek,
        [EnumMember(Value = "GettingWarmerNextWeek")] GettingWarmerNextWeek,
        [EnumMember(Value = "NextWeekAlmostNoPrecipitation")] NextWeekAlmostNoPrecipitation,
        [EnumMember(Value = "NextWeekPrecipitation")] NextWeekPrecipitation,
        [EnumMember(Value = "TemperatureNextWeek")] TemperatureNextWeek,
        [EnumMember(Value = "TodayPrecipitation")] TodayPrecipitation,
        [EnumMember(Value = "TodayDrySoil")] TodayDrySoil,
        [EnumMember(Value = "TodayWindAndGust")] TodayWindAndGust,
        [EnumMember(Value = "TodayPrecipitationWillBegin")] TodayPrecipitationWillBegin,
        [EnumMember(Value = "TodayPrecipitationWillBeginAndEnd")] TodayPrecipitationWillBeginAndEnd,
        [EnumMember(Value = "TodayPrecipitationWillEnd")] TodayPrecipitationWillEnd,
        [EnumMember(Value = "TodayPrecipitationWontEnd")] TodayPrecipitationWontEnd,
        [EnumMember(Value = "TodayTemperatureIsLowerThanYesterday")] TodayTemperatureIsLowerThanYesterday,
        [EnumMember(Value = "TodayTemperatureIsTheSameAsYesterday")] TodayTemperatureIsTheSameAsYesterday,
        [EnumMember(Value = "TomorrowWindAndGust")] TomorrowWindAndGust,
        [EnumMember(Value = "TomorrowDayPartPrecipitation")] TomorrowDayPartPrecipitation,
        [EnumMember(Value = "TomorrowMostlyPrecipitation")] TomorrowMostlyPrecipitation,
        [EnumMember(Value = "WeekendMostlyPrecipitation")] WeekendMostlyPrecipitation,
        [EnumMember(Value = "TemperatureChangesNextWeek")] TemperatureChangesNextWeek,
        [EnumMember(Value = "TomorrowTemperatureIsTheSameAsToday")] TomorrowTemperatureIsTheSameAsToday,
        [EnumMember(Value = "TomorrowTemperatureIsLowerThanToday")] TomorrowTemperatureIsLowerThanToday,
        [EnumMember(Value = "WeekendTemperatureWillBeHigherThanToday")] WeekendTemperatureWillBeHigherThanToday,
        [EnumMember(Value = "WeekendTemperatureWillBeTheSameAsToday")] WeekendTemperatureWillBeTheSameAsToday,
        [EnumMember(Value = "TodayNoPrecipitation")] TodayNoPrecipitation,
        [EnumMember(Value = "TodayPressureWillIncrease")] TodayPressureWillIncrease,
        [EnumMember(Value = "TomorrowTemperatureIsHigherThanToday")] TomorrowTemperatureIsHigherThanToday,
        [EnumMember(Value = "NextWeekWeakWind")] NextWeekWeakWind,
        [EnumMember(Value = "NextWeekNoPrecipitation")] NextWeekNoPrecipitation,
        [EnumMember(Value = "NextWeekPressureWillDrop")] NextWeekPressureWillDrop,
        [EnumMember(Value = "NextWeekPressureWillIncrease")] NextWeekPressureWillIncrease,
        [EnumMember(Value = "WeekendNoPrecipitation")] WeekendNoPrecipitation,
        [EnumMember(Value = "WeekendPressureWillIncrease")] WeekendPressureWillIncrease,
        [EnumMember(Value = "TodayTemperature")] TodayTemperature,
        [EnumMember(Value = "TomorrowNoPrecipitation")] TomorrowNoPrecipitation
    }

    public enum WarningTitle
    {
        [EnumMember(Value = "В выходные")] Weekend,
        [EnumMember(Value = "Завтра")] Tomorrow,
        [EnumMember(Value = "На следующей неделе")] NextWeek,
        [EnumMember(Value = "Сегодня")] Today,
        [EnumMember(Value = "")] Empty
    }

    public enum PollenLevel
    {
        [EnumMember(Value = "clear")] Clear,
        [EnumMember(Value = "normal")] Normal,
        [EnumMember(Value = "strong")] Strong,
        [EnumMember(Value = "weak")] Weak
    }
}
