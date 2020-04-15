using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;



namespace IDGFileSystemWatcher

{

    class Program

    {

        static void Main(string[] args)

        {


            string path = @"D:\Users\brecht\Documents\TestmapNET";

            MonitorDirectory(path);

            Console.ReadKey();

        }

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
            string[,] array2Da = new string[2,32];
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
            string[] lines = System.IO.File.ReadAllLines(path);
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
            String sql = "";

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
    }
}