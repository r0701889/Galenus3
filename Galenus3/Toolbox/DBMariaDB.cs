using MySql.Data.MySqlClient;
using ConsoleAppBelimed.JULIETClasses;
using System;

namespace ConsoleAppBelimed.Toolbox
{
    public static class DBMariaDB
    {
        public static MySqlConnection GetConnection(string aDatabase, string aServer, string aUserID, string aPassword)
        {
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Database = aDatabase,
                Server = aServer,
                UserID = aUserID,
                Password = aPassword
            };
            MySqlConnection connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
            if (connection != null) { connection.Open(); }
            return connection;
        }

        public static bool Insert(JuMachineDataBelimed aMachineDataBelimed, MySqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINEDATABELIMED (MACHINEID, MACHINENAME, PROGRAMREFERENCE, PROGRAMNAME, STARTEDBY, SOFTWAREVERSION, " +
                "LASTBDTEST, LASTBDTESTREFERENCE, LASTVACUUMTEST, LASTVACUUMTESTREFERENCE, CYCLEREFERENCE, CYCLESTARTED, CYCLEENDED, ISCYCLEOK, CYCLERESULT, " +
                "HOLDTIME, F0VALUE, PROGRAMDATE, CYCLEDURATION, STERILIZATIONTEMPMIN, STERILIZATIONTEMPMAX, LEAKRATE, FUNCTIONALCHECK, " +
                "CYCLERELEASED, CYCLERELEASEDBY, CYCLERELEASESTATE, BATCHDATA1, BATCHDATA2, BATCHDATA3, TESTRESULT, RIF1, RIF2, RIF3, DTCREATED) " +
                "VALUES(@MACHINEID, @MACHINENAME, @PROGRAMREFERENCE, @PROGRAMNAME, @STARTEDBY, @SOFTWAREVERSION, " +
                "@LASTBDTEST, @LASTBDTESTREFERENCE, @LASTVACUUMTEST, @LASTVACUUMTESTREFERENCE, @CYCLEREFERENCE, @CYCLESTARTED, @CYCLEENDED, @ISCYCLEOK, @CYCLERESULT, " +
                "@HOLDTIME, @F0VALUE, @PROGRAMDATE, @CYCLEDURATION, @STERILIZATIONTEMPMIN, @STERILIZATIONTEMPMAX, @LEAKRATE, @FUNCTIONALCHECK, " +
                "@CYCLERELEASED, @CYCLERELEASEDBY, @CYCLERELEASESTATE, @BATCHDATA1, @BATCHDATA2, @BATCHDATA3, @TESTRESULT, @RIF1, @RIF2, @RIF3, @DTCREATED)", aSchema);
            int rowCount = 0;

            using (MySqlCommand cmd = new MySqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MACHINEID", aMachineDataBelimed.MachineID);
                cmd.Parameters.AddWithValue("@MACHINENAME", aMachineDataBelimed.MachineName);
                cmd.Parameters.AddWithValue("@PROGRAMREFERENCE", aMachineDataBelimed.ProgramReference);
                cmd.Parameters.AddWithValue("@PROGRAMNAME", aMachineDataBelimed.ProgramName);
                cmd.Parameters.AddWithValue("@STARTEDBY", aMachineDataBelimed.StartedBy);
                cmd.Parameters.AddWithValue("@SOFTWAREVERSION", aMachineDataBelimed.SoftwareVersion);
                cmd.Parameters.AddWithValue("@LASTBDTEST", aMachineDataBelimed.LastBDTest);
                cmd.Parameters.AddWithValue("@LASTBDTESTREFERENCE", aMachineDataBelimed.LastBDTestReference);
                cmd.Parameters.AddWithValue("@LASTVACUUMTEST", aMachineDataBelimed.LastVacuumTest);
                cmd.Parameters.AddWithValue("@LASTVACUUMTESTREFERENCE", aMachineDataBelimed.LastVacuumTestReference);
                cmd.Parameters.AddWithValue("@CYCLEREFERENCE", aMachineDataBelimed.CycleReference);
                cmd.Parameters.AddWithValue("@CYCLESTARTED", aMachineDataBelimed.CycleStarted);
                cmd.Parameters.AddWithValue("@CYCLEENDED", aMachineDataBelimed.CycleEnded);
                cmd.Parameters.AddWithValue("@ISCYCLEOK", Conversions.BoolToString(aMachineDataBelimed.IsCycleOk));
                cmd.Parameters.AddWithValue("@CYCLERESULT", aMachineDataBelimed.CycleResult);
                cmd.Parameters.AddWithValue("@HOLDTIME", aMachineDataBelimed.HoldTime);
                cmd.Parameters.AddWithValue("@F0VALUE", aMachineDataBelimed.F0Value);
                cmd.Parameters.AddWithValue("@PROGRAMDATE", aMachineDataBelimed.ProgramDate);
                cmd.Parameters.AddWithValue("@CYCLEDURATION", aMachineDataBelimed.CycleDuration);
                cmd.Parameters.AddWithValue("@STERILIZATIONTEMPMIN", aMachineDataBelimed.SterilizationTempMin);
                cmd.Parameters.AddWithValue("@STERILIZATIONTEMPMAX", aMachineDataBelimed.SterilizationTempMax);
                cmd.Parameters.AddWithValue("@LEAKRATE", aMachineDataBelimed.LeakRate);
                cmd.Parameters.AddWithValue("@FUNCTIONALCHECK", aMachineDataBelimed.FunctionalCheck);
                cmd.Parameters.AddWithValue("@CYCLERELEASED", aMachineDataBelimed.CycleReleased);
                cmd.Parameters.AddWithValue("@CYCLERELEASEDBY", aMachineDataBelimed.CycleReleasedBy);
                cmd.Parameters.AddWithValue("@CYCLERELEASESTATE", aMachineDataBelimed.CycleReleaseState);
                cmd.Parameters.AddWithValue("@BATCHDATA1", aMachineDataBelimed.BatchData1);
                cmd.Parameters.AddWithValue("@BATCHDATA2", aMachineDataBelimed.BatchData2);
                cmd.Parameters.AddWithValue("@BATCHDATA3", aMachineDataBelimed.BatchData3);
                cmd.Parameters.AddWithValue("@TESTRESULT", aMachineDataBelimed.TestResult);
                cmd.Parameters.AddWithValue("@RIF1", aMachineDataBelimed.RIF1);
                cmd.Parameters.AddWithValue("@RIF2", aMachineDataBelimed.RIF2);
                cmd.Parameters.AddWithValue("@RIF3", aMachineDataBelimed.RIF3);
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }

        public static bool Insert(JuMachineSensor aMachineSensor, long aMDNDX, MySqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINESENSOR (MDNDX, SENSORID, CAPTION, SENSORTYPE, SENSORUNIT, DTCREATED) " +
                "VALUES(@MDNDX, @SENSORID, @CAPTION, @SENSORTYPE, @SENSORUNIT, @DTCREATED)", aSchema);
            int rowCount = 0;

            using (MySqlCommand cmd = new MySqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MDNDX", aMDNDX);
                cmd.Parameters.AddWithValue("@SENSORID", aMachineSensor.SensorID);
                cmd.Parameters.AddWithValue("@CAPTION", aMachineSensor.Caption);
                cmd.Parameters.AddWithValue("@SENSORTYPE", (int)aMachineSensor.SensorType);
                cmd.Parameters.AddWithValue("@SENSORUNIT", (int)aMachineSensor.SensorUnit);
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }

        public static bool Insert(JuMachineSensorValue aMachineSensorValue, MySqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINESENSORVALUE (MDNDX, DTARGUMENT, STAGE, PHASE, MESSAGEREFERENCE, " +
                "MESSAGE, SENSOR1, SENSOR2, SENSOR3, SENSOR4, SENSOR5, SENSOR6, ZONEREFERENCE, DTCREATED) " +
                "VALUES(@MDNDX, @DTARGUMENT, @STAGE, @PHASE, @MESSAGEREFERENCE, " +
                "@MESSAGE, @SENSOR1, @SENSOR2, @SENSOR3, @SENSOR4, @SENSOR5, @SENSOR6, @ZONEREFERENCE, @DTCREATED)", aSchema);
            int rowCount = 0;

            using (MySqlCommand cmd = new MySqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MDNDX", aMachineSensorValue.MDNDX);
                cmd.Parameters.AddWithValue("@DTARGUMENT", aMachineSensorValue.DTArgument);
                cmd.Parameters.AddWithValue("@STAGE", aMachineSensorValue.Stage);
                cmd.Parameters.AddWithValue("@PHASE", aMachineSensorValue.Phase);
                cmd.Parameters.AddWithValue("@MESSAGEREFERENCE", aMachineSensorValue.MessageReference);
                cmd.Parameters.AddWithValue("@MESSAGE", aMachineSensorValue.Message);
                cmd.Parameters.AddWithValue("@SENSOR1", aMachineSensorValue.Sensor1);
                cmd.Parameters.AddWithValue("@SENSOR2", aMachineSensorValue.Sensor2);
                cmd.Parameters.AddWithValue("@SENSOR3", aMachineSensorValue.Sensor3);
                cmd.Parameters.AddWithValue("@SENSOR4", aMachineSensorValue.Sensor4);
                cmd.Parameters.AddWithValue("@SENSOR5", aMachineSensorValue.Sensor5);
                cmd.Parameters.AddWithValue("@SENSOR6", aMachineSensorValue.Sensor6);
                cmd.Parameters.AddWithValue("@ZONEREFERENCE", aMachineSensorValue.ZoneReference);
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }

        public static bool Select(JuMachineDataBelimed aMachineDataBelimed, MySqlConnection aConnection, string aSchema, out long aMDNDX)
        {
            aMDNDX = 0;
            string sql = string.Format("SELECT MDNDX FROM {0}.ORIS_MACHINEDATABELIMED WHERE MACHINEID = @MACHINEID AND CYCLEREFERENCE = @CYCLEREFERENCE", aSchema);
            string mdNDXString = "";

            using (MySqlCommand cmd = new MySqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MACHINEID", aMachineDataBelimed.MachineID);
                cmd.Parameters.AddWithValue("@CYCLEREFERENCE", aMachineDataBelimed.CycleReference);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mdNDXString = reader["MDNDX"].ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(mdNDXString)) { return false; }
            if (!long.TryParse(mdNDXString, out aMDNDX)) { return false; }
            return aMDNDX > 0;
        }
    }
}
