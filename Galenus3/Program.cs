using System;
using System.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using ConsoleAppBelimed.JULIETClasses;
using ConsoleAppBelimed.Toolbox;

namespace ConsoleAppBelimed

{
    class Program
    {
        private static string SourceFolder { get; set; }
        private static string ArchiveFolder { get; set; }
        private static bool ArchiveFile { get; set; }
        private static string Database { get; set; }
        private static string Schema { get; set; }
        private static string Server { get; set; }
        private static string UserID { get; set; }
        private static string Password { get; set; }

        private const string FileFilter = "K*.csv";

        static void Main()
        {
            if (ConfigureApplication())
            {
                using (FileSystemWatcher watcher = new FileSystemWatcher())
                {
                    // Set properties of watcher
                    watcher.Path = SourceFolder;
                    watcher.Filter = FileFilter;
                    watcher.IncludeSubdirectories = true;

                    // Set eventhandler of watcher
                    watcher.Created += new FileSystemEventHandler(Oncreated);
                    watcher.EnableRaisingEvents = true;

                    // Enable the user to quit the program
                    Console.WriteLine("Monitoring: " + SourceFolder);
                    Console.WriteLine("Press 'q' to quit the application.");
                    while (Console.Read() != 'q') ;
                }
            }
            else
            {
                Console.WriteLine("Press any key to quit the application.");
                Console.Read();
            }
        }

        private static void Oncreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("New file detected: " + e.Name);
            MySqlConnection connection = null;
            try
            {
                connection = DBMariaDB.GetConnection(Database, Server, UserID, Password);
                if (connection != null)
                {
                    if (!DBMariaDB.GetMDNDX(connection, Schema, out long mdNDX))
                    {
                        Console.WriteLine(e.Name + ": could not get a valid MDNDX");
                        return;
                    }
                    JuMachineDataBelimed machineDataBelimed = new JuMachineDataBelimed(mdNDX);
                    if (machineDataBelimed.LoadFromFile(e.FullPath))
                    {
                        Console.WriteLine(e.Name + " was parsed.");
                    }
                    else
                    {
                        Console.WriteLine(e.Name + " could not be parsed.");
                        return;
                    }
                    if (!DBMariaDB.Insert(machineDataBelimed, connection, Schema))
                    {
                        Console.WriteLine(e.Name + ": MachineData could not be written to database");
                        return;
                    }
                    foreach (JuMachineSensor machineSensor in machineDataBelimed.MachineSensors)
                    {
                        if (!DBMariaDB.Insert(machineSensor, mdNDX, connection, Schema))
                        {
                            Console.WriteLine(e.Name + ": MachineSensor " + machineSensor.SensorID + " could not be written to database");
                            return;
                        }
                    }
                    // TODO: save MachineSensorValues to database, look at the foreach above as an example
                }
                else
                {
                    Console.WriteLine("Connection to database failed");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(e.Name + " database handling error: " + exc.Message);
            }
            finally
            {
                if (connection != null) { connection.Close(); }
            }

            if (ArchiveFile)
            {
                if (!MoveFileToArchive(e.FullPath))
                {
                    Console.WriteLine(e.Name + " was archived.");
                }
                else
                {
                    Console.WriteLine(e.Name + " could not be archived.");
                }
            }
        }

        private static bool MoveFileToArchive(string aFileFullPath)
        {
            if (!File.Exists(aFileFullPath)) { return false; }

            // TODO: write function
            return true;
        }

        private static bool ConfigureApplication()
        {
            try
            {
                SourceFolder = ConfigurationManager.AppSettings.Get("SourceFolder");
                if (!Directory.Exists(SourceFolder))
                {
                    Console.WriteLine(SourceFolder + " does not exists.");
                    return false;
                }
                ArchiveFolder = ConfigurationManager.AppSettings.Get("ArchiveFolder");
                ArchiveFile = !string.IsNullOrEmpty(ArchiveFolder);
                if (ArchiveFile && (!Directory.Exists(ArchiveFolder)))
                {
                    Console.WriteLine(ArchiveFolder + " does not exists.");
                    return false;
                }
                Database = ConfigurationManager.AppSettings.Get("Database");
                Schema = ConfigurationManager.AppSettings.Get("Schema");
                Server = ConfigurationManager.AppSettings.Get("Server");
                UserID = ConfigurationManager.AppSettings.Get("UserID");
                Password = ConfigurationManager.AppSettings.Get("Password");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ConfigureApplication: " + e.Message);
            }
            return false;
        }

        /*

        private static void MonitorDirectory(string path)

        {

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;


        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var BestandNaam = e.Name;

            Console.WriteLine("File created: {0}", BestandNaam);

            string s = BestandNaam.Substring(0, 1);

            if (s == "K")
            {
                LeesKFileUit(BestandNaam);

            }
            else if (s == "C")
            {

                LeesCFileUit(BestandNaam);
            }
            else
            {
                Console.WriteLine("Unknown file, cant be processed");
            }

        }

        private static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File renamed: {0}", e.Name);
        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File deleted: {0}", e.Name);
        }

        private static void LeesKFileUit(string BestandNaam)
        {

            //initialisatie van mijn variable en mijn folder map
            var reader = new StreamReader(File.OpenRead(@"D:\Users\brecht\Documents\TestmapNET\" + BestandNaam));
            string[,] array2Da = new string[2, 32];
            string[,] resultaat = new string[32, 2];
            int teller = 0;

            // hier lees ik mijn .csv bestant per line.
            while (!reader.EndOfStream)
            {

                var line = reader.ReadLine();
                var values = line.Split(';');

                for (int i = 0; i < 32; i++)
                {
                    array2Da[teller, i] = values[i];
                }

                teller++;
            }

            // de variable array2Da is nu nog geordend via rij, in deze code zet ik de header en waarden samen zodat het duidelijker is.
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    resultaat[j, i] = array2Da[i, j];
                }
            }

            //nu steek ik de array "resultaat" in de database
            InsertInDatabase(resultaat);
        }
        private static void LeesCFileUit(string BestandNaam)
        {

            var reader = new StreamReader(File.OpenRead(@"D:\Users\brecht\Documents\TestmapNET\" + BestandNaam));

            string path = "D:/Users/brecht/Documents/TestmapNET/" + BestandNaam;
            string[] lines = File.ReadAllLines(path);
            string[,] array2Da = new string[lines.Length, 14];

            int teller = 0;

            // hier lees ik mijn .csv bestant per line.
            while (!reader.EndOfStream)
            {

                var line = reader.ReadLine();
                var values = line.Split(';');

                for (int i = 0; i < 14; i++)
                {
                    array2Da[teller, i] = values[i];
                }

                teller++;
            }


        }

        private static void InsertInDatabase(string[,] array2Da)
        {

            string connetionString = null;
            connetionString = "Data Source=192.168.0.215; Initial Catalog=Galenus;User ID=brecht1;Password=brecht1";

            MySqlCommand command;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string sql = "";

            MySqlConnection conn = new MySqlConnection(connetionString);


            conn.Open();
            Console.WriteLine("Connection Open !");

            sql = "INSERT INTO `Run_info_galenus` (`N° machine`, `N° cyclus`, `Gebruiker`, `Start_cyclus`, `Stop_cyclus`, `Status programma`, `N° programma`, `Naam programma`, `duurtijd cyclus`, `Duur sterilisatie`, `Min temp`, `Max temp`, `Machine_naam`)  VALUES ('" + array2Da[0, 1] + "', '" + array2Da[1, 1] + "', '" + array2Da[2, 1] + "', '" + array2Da[3, 1] + "', '" + array2Da[4, 1] + "', '" + array2Da[5, 1] + "', '" + array2Da[6, 1] + "', '" + array2Da[7, 1] + "', '" + array2Da[10, 1] + "', '" + array2Da[11, 1] + "', '" + array2Da[12, 1] + "', '" + array2Da[13, 1] + "', '" + array2Da[27, 1] + "')";

            command = new MySqlCommand(sql, conn);
            adapter.InsertCommand = new MySqlCommand(sql, conn);
            adapter.InsertCommand.ExecuteNonQuery();

            command.Dispose();

            conn.Close();

        }
         */
    }

}