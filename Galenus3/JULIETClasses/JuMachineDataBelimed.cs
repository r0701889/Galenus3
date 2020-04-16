using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConsoleAppBelimed.Toolbox;


namespace ConsoleAppBelimed.JULIETClasses
{
    public class JuMachineDataBelimed : JuMachineData
    {
        public DateTime ProgramDate { get; private set; } = DateTime.MinValue;
        public int CycleDuration { get; private set; } = 0; // unit: seconds
        public double SterilizationTempMin { get; private set; } = 0.0; // unit: degrees C
        public double SterilizationTempMax { get; private set; } = 0.0; // unit: degrees C
        public double LeakRate { get; private set; } = 0.0; // unit: mbar/min
        public string FunctionalCheck { get; private set; } = "";
        public DateTime CycleReleased { get; private set; } = DateTime.MinValue;
        public string CycleReleasedBy { get; private set; } = "";
        public string CycleReleaseState { get; private set; } = "";
        public string BatchData1 { get; private set; } = "";
        public string BatchData2 { get; private set; } = "";
        public string BatchData3 { get; private set; } = "";
        public string TestResult { get; private set; } = "";
        public string RIF1 { get; private set; } = "";
        public string RIF2 { get; private set; } = "";
        public string RIF3 { get; private set; } = "";

        public override JuMachineInterfaceType MachineInterfaceType => JuMachineInterfaceType.Belimed;
        public override string Manufacturer => "Belimed AG";

        private const string DataDelimiter = ";";
        private const int LineCount = 2;
        private const int KFileFieldCount = 32;
        private const int CFileFieldCount = 14;
        private const int SensorCount = 6;

        public JuMachineDataBelimed(long aMDNDX) : base(aMDNDX) { }

        public override bool LoadFromFile(string aFileFullPath)
        {
            try
            {
                if (!LoadFromKFile(aFileFullPath)) { return false; }
                if (!LoadFromCFile(aFileFullPath)) { return false; }
                return true;
            }
            catch (MalformedLineException e)
            {
                Console.WriteLine("JuMachineDataBelimed.LoadFromFile: " + aFileFullPath + ": malformed line " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("JuMachineDataBelimed.LoadFromFile: " + aFileFullPath + ": " + e.Message);
            }
            return false;
        }

        private bool LoadFromKFile(string aKFileFullPath)
        {
            if (!File.Exists(aKFileFullPath)) { return false; }
            if (!Conversions.CheckFileName(aKFileFullPath, MachineInterfaceType)) { return false; }
            
            using (TextFieldParser csvParser = new TextFieldParser(aKFileFullPath, Encoding.GetEncoding("iso-8859-1")))
            {
                // Setup parser
                csvParser.SetDelimiters(new string[] { DataDelimiter });
                csvParser.HasFieldsEnclosedInQuotes = true;

                int lineCnt = 0;
                while ((!csvParser.EndOfData) && (lineCnt < LineCount))
                {
                    // Read current line fields - pointer moves to next line
                    string[] fields = csvParser.ReadFields();
                    lineCnt++;
                    if (fields.Length < KFileFieldCount)
                    {
                        Console.WriteLine("JuMachineDataBelimed.LoadFromkFile: " + aKFileFullPath + " line " + lineCnt.ToString() + " not enough values");
                        return false;
                    }
                    if (lineCnt > 1)
                    {
                        return ParseKFile(fields);
                    }
                }
            }
            return false;
        }
        private bool LoadFromCFile(string aKFileFullPath)
        {
            if (!File.Exists(aKFileFullPath)) { return false; }
            string cFileFullPath = GetCFilePath(aKFileFullPath);
            if (!File.Exists(cFileFullPath)) { return false; }
            if (!Conversions.CheckFileName(cFileFullPath, MachineInterfaceType)) { return false; }
            
            AddMachineSensors();
            MachineSensorValues = new List<JuMachineSensorValue>();

            using (TextFieldParser csvParser = new TextFieldParser(cFileFullPath, Encoding.GetEncoding("iso-8859-1")))
            {
                // Setup parser
                csvParser.SetDelimiters(new string[] { DataDelimiter });
                csvParser.HasFieldsEnclosedInQuotes = true;

                int lineCnt = 0;
                while (!csvParser.EndOfData)
                {
                    // Read current line fields - pointer moves to next line
                    string[] fields = csvParser.ReadFields();
                    lineCnt++;
                    if (fields.Length < CFileFieldCount)
                    {
                        Console.WriteLine("JuMachineDataBelimed.LoadFromCFile: " + cFileFullPath + " line " + lineCnt.ToString() + " not enough values");
                        return false;
                    }
                    if (lineCnt > 1)
                    {
                        if (!ParseCFile(fields, out JuMachineSensorValue machineSensorValue) || (machineSensorValue == null)) { return false; }
                        MachineSensorValues.Add(machineSensorValue);
                    }
                }
            }
            return true;
        }

        private bool ParseKFile(string[] aFields)
        {
            if ((aFields == null) || (aFields.Length != KFileFieldCount)) { return false; }
            
            MachineID = aFields[0];
            CycleReference = aFields[1];
            StartedBy = aFields[2];
            CycleStarted = Conversions.StringToDateTime(aFields[3]);
            CycleEnded = Conversions.StringToDateTime(aFields[4]);
            IsCycleOk = Conversions.StringToCycleResult(aFields[5], out string cycleResult);
            CycleResult = cycleResult;
            ProgramReference = aFields[6];
            ProgramName = aFields[7];
            ProgramDate = Conversions.StringToDate(aFields[8]);
            SoftwareVersion = aFields[9];
            CycleDuration = Conversions.StringToInt(aFields[10]);
            HoldTime = Conversions.StringToInt(aFields[11]);
            SterilizationTempMin = Conversions.StringToDouble(aFields[12]);
            SterilizationTempMax = Conversions.StringToDouble(aFields[13]);
            F0Value = Conversions.StringToDouble(aFields[14]);
            LeakRate = Conversions.StringToDouble(aFields[15]);
            LastBDTest = Conversions.StringToDateTime(aFields[16]);
            LastBDTestReference = aFields[17];
            LastVacuumTest = Conversions.StringToDateTime(aFields[18]);
            LastVacuumTestReference = aFields[19];
            FunctionalCheck = aFields[20];
            CycleReleasedBy = aFields[21];
            CycleReleased = Conversions.StringToDateTime(aFields[22]);
            CycleReleaseState = aFields[23];
            BatchData1 = aFields[24];
            BatchData2 = aFields[25];
            BatchData3 = aFields[26];
            MachineName = aFields[27];
            TestResult = aFields[28];
            RIF1 = aFields[29];
            RIF2 = aFields[30];
            RIF3 = aFields[31];
            return true;
        }

        private bool ParseCFile(string[] aFields, out JuMachineSensorValue aMachineSensorValue)
        {
            aMachineSensorValue = null;
            if ((aFields == null) || (aFields.Length != CFileFieldCount)) { return false; }
            
            string machineID = aFields[0];
            if (string.IsNullOrEmpty(machineID) || (!string.Equals(machineID, MachineID))) { return false; }
            string cycleReference = aFields[1];
            if (string.IsNullOrEmpty(cycleReference) || (!string.Equals(cycleReference, CycleReference))) { return false; }
            DateTime dtArgument = Conversions.StringToDateTime(aFields[2], CycleStarted);
            string stage = aFields[3];
            string phase = aFields[4];
            string messageReference = aFields[5];
            string message = aFields[6];
            double sensor1 = Conversions.StringToDouble(aFields[7]);
            double sensor2 = Conversions.StringToDouble(aFields[8]);
            double sensor3 = Conversions.StringToInt(aFields[9]);
            double sensor4 = Conversions.StringToInt(aFields[10]);
            double sensor5 = Conversions.StringToInt(aFields[11]);
            double sensor6 = Conversions.StringToInt(aFields[12]);
            string zoneReference = aFields[13];
            aMachineSensorValue = new JuMachineSensorValue(MDNDX, dtArgument, stage, phase, messageReference, message, sensor1, sensor2, sensor3, sensor4, sensor5, sensor6, zoneReference);
            return true;
        }

        private void AddMachineSensors()
        {
            MachineSensors = new List<JuMachineSensor>();
            for (int i = 0; i <= SensorCount; i++)
            {
                MachineSensors.Add(new JuMachineSensor(MDNDX, i, JuMachineSensor.GetCaption(i), JuMachineSensor.GetSensorType(i), JuMachineSensor.GetSensorUnit(i)));
            }
        }

        private string GetCFilePath(string aKFileFullPath)
        {
            if (string.IsNullOrEmpty(aKFileFullPath)) { return ""; }

            string kFileName = Path.GetFileName(aKFileFullPath);
            if (string.IsNullOrEmpty(kFileName)) { return ""; }
            string folder = Path.GetDirectoryName(aKFileFullPath);
            if (string.IsNullOrEmpty(folder)) { return ""; }

            string cFileName = "C" + kFileName.Substring(1);
            return Path.Combine(folder, cFileName);
        }
    }
}
