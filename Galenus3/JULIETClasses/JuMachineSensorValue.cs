using System;

namespace ConsoleAppBelimed.JULIETClasses
{
    public class JuMachineSensorValue
    {
        public long MDNDX { get; private set; }
        public DateTime DTArgument { get; private set; }
        public string Stage { get; private set; }
        public string Phase { get; private set; }
        public string MessageReference { get; private set; }
        public string Message { get; private set; }
        public double Sensor1 { get; private set; }
        public double Sensor2 { get; private set; }
        public double Sensor3 { get; private set; }
        public double Sensor4 { get; private set; }
        public double Sensor5 { get; private set; }
        public double Sensor6 { get; private set; }
        public string ZoneReference { get; private set; }

        public JuMachineSensorValue(
            long aMDNDX,
            DateTime aDTArgument,
            string aStage,
            string aPhase,
            string aMessageReference,
            string aMessage,
            double aSensor1,
            double aSensor2,
            double aSensor3,
            double aSensor4,
            double aSensor5,
            double aSensor6,
            string aZoneReference)
        {
            MDNDX = aMDNDX;
            DTArgument = aDTArgument;
            Stage = aStage;
            Phase = aPhase;
            MessageReference = aMessageReference;
            Message = aMessage;
            Sensor1 = aSensor1;
            Sensor2 = aSensor2;
            Sensor3 = aSensor3;
            Sensor4 = aSensor4;
            Sensor5 = aSensor5;
            Sensor6 = aSensor6;
            ZoneReference = aZoneReference;
        }
    }
}
