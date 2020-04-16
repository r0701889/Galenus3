using ConsoleAppBelimed.Toolbox;

namespace ConsoleAppBelimed.JULIETClasses
{
    public class JuMachineSensor
    {
        public long MDNDX { get; }
        public int SensorID { get; }
        public string Caption { get; }
        public JuSensorType SensorType { get; }
        public JuSensorUnit SensorUnit { get; }

        public JuMachineSensor(
                long aMDNDX,
                int aSensorID,
                string aCaption,
                JuSensorType aSensorType,
                JuSensorUnit aSensorUnit)
        {
            MDNDX = aMDNDX;
            SensorID = aSensorID;
            Caption = aCaption;
            SensorType = aSensorType;
            SensorUnit = aSensorUnit;
        }

        public static string GetCaption(int aIndex)
        {
            switch (aIndex)
            {
                default: return "";
                case 0: return "Time";
                case 1: return "T1";
                case 2: return "T2";
                case 3: return "P1";
                case 4: return "P2";
                case 5: return "P3";
                case 6: return "P4";
            }
        }

        public static JuSensorType GetSensorType(int aIndex)
        {
            switch (aIndex)
            {
                default: 
                    return JuSensorType.NotDefined;
                case 0: 
                    return JuSensorType.Time;
                case 1: 
                case 2: 
                    return JuSensorType.Double;
                case 3:
                case 4:
                case 5:
                case 6:
                    return JuSensorType.Integer;
            }
        }

        public static JuSensorUnit GetSensorUnit(int aIndex)
        {
            switch (aIndex)
            {
                default:
                case 0:
                    return JuSensorUnit.NotDefined;
                case 1:
                case 2:
                    return JuSensorUnit.DegreesCelsius;
                case 3:
                case 4:
                case 5:
                case 6:
                    return JuSensorUnit.MBarA;
            }
        }
    }
}
