﻿using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleAppBelimed.Toolbox
{
    public enum JuMachineInterfaceType
    {
        NotDefined = 0,
        Belimed = 1,
        MMM = 2
    }

    public enum JuSensorType
    {
        NotDefined = 0,
        Integer = 1,
        Double = 2,
        String = 3,
        Time = 4
    }

    public enum JuSensorUnit
    {
        NotDefined = 0,
        DegreesCelsius = 1, // °C
        MBar = 2, // mbar
        MBarA = 3, // mbar a
        Min = 4 // min
    }

    public static class Conversions
    {
        private const string RegExBelimed = @"^[CK]_\d+_\d+$";
        private const string RegExMMM = @"^\d{7}[+-]$";

        public static bool CheckFileName(string aFileFullPath, JuMachineInterfaceType aMachineInterfaceType)
        {
            if (string.IsNullOrEmpty(aFileFullPath)) { return false; }
            string fileName = Path.GetFileNameWithoutExtension(aFileFullPath);
            if (string.IsNullOrEmpty(fileName)) { return false; }
            switch (aMachineInterfaceType)
            {
                default:
                case JuMachineInterfaceType.NotDefined:
                    return false;
                case JuMachineInterfaceType.Belimed:
                    return CheckFileNameBelimed(fileName);
                case JuMachineInterfaceType.MMM:
                    return CheckFileNameMMM(fileName);
            }
        }

        public static string BoolToString(bool aBool)
        {
            return aBool ? "T" : "F";
        }

        public static bool StringToCycleResult(string aCycleResultString, out string aCycleResult)
        {
            aCycleResult = "";
            if (string.IsNullOrEmpty(aCycleResultString)) { return false; }
            if (aCycleResultString.Length < 3) { return false; }
            aCycleResult = aCycleResultString.Substring(2);
            return StringToBool(aCycleResultString.Substring(0, 1));
        }

        public static bool StringToBool(string aBoolString)
        {
            if (string.IsNullOrEmpty(aBoolString)) { return false; }
            aBoolString = aBoolString.ToLower();
            return aBoolString.Equals("true") || aBoolString.Equals("0");
        }

        // Date fornat: dd.MM.yyyy
        public static DateTime StringToDate(string aDateTime)
        {
            if (string.IsNullOrEmpty(aDateTime)) { return DateTime.MinValue; }
            if (DateTime.TryParseExact(aDateTime, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) { return date; }
            return DateTime.MinValue;
        }

        // DateTime format: yyyy-MM-dd-HH.mm.ss.ffffff
        public static DateTime StringToDateTime(string aDateTimeString)
        {
            if (string.IsNullOrEmpty(aDateTimeString)) { return DateTime.MinValue; }
            if (DateTime.TryParseExact(aDateTimeString, "yyyy-MM-dd-HH.mm.ss.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime)) { return dateTime; }
            return DateTime.MinValue;
        }

        // Time format: HH:m:ss
        public static DateTime StringToDateTime(string aTimeString, DateTime aDate)
        {
            if (string.IsNullOrEmpty(aTimeString)) { return DateTime.MinValue; }
            if (aDate == DateTime.MinValue || aDate == DateTime.MaxValue) { return DateTime.MinValue; }
            if (!DateTime.TryParseExact(aTimeString, "HH:m:ss", null, DateTimeStyles.None, out DateTime time)) { return DateTime.MinValue; }
            return new DateTime(aDate.Year, aDate.Month, aDate.Day, time.Hour, time.Minute, time.Second);
        }

        public static double StringToDouble(string aDoubleString)
        {
            if (string.IsNullOrEmpty(aDoubleString)) { return 0.0; }
            aDoubleString = aDoubleString.Replace(',', '.');
            if (double.TryParse(aDoubleString, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedDouble)) { return parsedDouble; }
            return 0.0;
        }

        public static int StringToInt(string aIntString)
        {
            if (string.IsNullOrEmpty(aIntString)) { return 0; }
            if (int.TryParse(aIntString, out int parsedInt)) { return parsedInt; }
            return 0;
        }

        public static JuSensorType StringToSensorType(string aSensorTypeAsString)
        {
            if (string.IsNullOrEmpty(aSensorTypeAsString)) { return JuSensorType.NotDefined; }
            switch (aSensorTypeAsString.ToLower())
            {
                default:
                    return JuSensorType.NotDefined;
                case "integer":
                    return JuSensorType.Integer;
                case "double":
                    return JuSensorType.Double;
                case "string":
                    return JuSensorType.String;
                case "time":
                    return JuSensorType.Time;
            }
        }

        public static JuSensorUnit StringToSensorUnit(string aSensorUnitAsString)
        {
            if (string.IsNullOrEmpty(aSensorUnitAsString)) { return JuSensorUnit.NotDefined; }
            switch (aSensorUnitAsString.ToLower())
            {
                default:
                    return JuSensorUnit.NotDefined;
                case "°c":
                    return JuSensorUnit.DegreesCelsius;
                case "mbar":
                    return JuSensorUnit.MBar;
                case "mbar a":
                    return JuSensorUnit.MBarA;
                case "min":
                    return JuSensorUnit.Min;
            }
        }

        private static bool CheckFileNameBelimed(string aFileName)
        {
            if (aFileName.Length < 5)
            {
                Console.WriteLine("CheckFileNameBelimed: " + aFileName + " file name too short");
                return false;
            }
            if (!Regex.IsMatch(aFileName, RegExBelimed))
            {
                Console.WriteLine("CheckFileNameBelimed: " + aFileName + " file name not has correct format");
                return false;
            }
            return true;
        }

        private static bool CheckFileNameMMM(string aFileName)
        {
            if (aFileName.Length != 8)
            {
                Console.WriteLine("CheckFileNameMMM: " + aFileName + " file name not has correct length");
                return false;
            }
            if (!Regex.IsMatch(aFileName, RegExMMM))
            {
                Console.WriteLine("CheckFileNameMMM: " + aFileName + " file name not has correct format");
                return false;
            }
            return true;
        }
    }
}
