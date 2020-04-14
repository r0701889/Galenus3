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

            bool isheader = true;
            var reader = new StreamReader(File.OpenRead(@"D:\Users\brecht\Documents\TestmapNET\" + BestandNaam));
            List<string> headers = new List<string>();

            var line = reader.ReadLine();
            var values = line.Split(';');
            string[,] array2Da = new string[values.Length, 2];

            while (!reader.EndOfStream)
            {


                if (isheader)
                {
                    isheader = false;
                    headers = values.ToList();
                }
                else
                {
                    int i = 0;
                    for (i = 0; i < values.Length; i++)
                    {

                        array2Da[i, 0] = headers[i];
                        array2Da[i, 1] = values[i];
                        //Console.WriteLine(string.Format("{0} = {1};", headers[i], values[i]));

                    }
                    
                    for (int w = 0; w < (array2Da.Length / 2); w++)
                    {
                        // Console.WriteLine("colom " + w + " :");
                        for (int d = 0; d < 2; d++)
                        {
                            //Console.WriteLine(array2Da[w, d]);
                        }
                    }

                }

            }

        }
        private static void LeesCFileUit(string BestandNaam)
        {

            var reader = new StreamReader(File.OpenRead(@"D:\Users\brecht\Documents\TestmapNET\" + BestandNaam));

            string path = "D:/Users/brecht/Documents/TestmapNET/" + BestandNaam;
            string[] lines = System.IO.File.ReadAllLines(path);
            string[,] array2Da = new string[lines.Length, 14];

            int plaats = 0;
            int invulplaats = 0;

            foreach (string line in lines)
            {
                string[] columns = line.Split(';');

                foreach (string column in columns)
                {
                    array2Da[plaats, invulplaats] = column;
                    invulplaats++;
                }
                invulplaats = 0;
                plaats++;
            }

            for (int w = 0; w < (lines.Length / 2); w++)
            {
                Console.WriteLine("colom " + w + " :");
                for (int d = 0; d < 14; d++)
                {
                    Console.WriteLine(array2Da[w, d]);
                }
            }

        }

        private static void InsertInDatabase(string[,] array2Da)
        {

            string connetionString = null;
            connetionString = "Data Source=192.168.0.211; Initial Catalog=Galenus;User ID=brecht1;Password=brecht1";

            MySqlCommand command;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            String sql = "";

            MySqlConnection conn = new MySqlConnection(connetionString);


            conn.Open();
            Console.WriteLine("Connection Open !");

            sql = "INSERT INTO Run_info_galenus (`N° machine`,`N° cyclus`,`Gebruiker`,`Start_cyclus`,`Stop_cyclus`,`Status programma`,`N° programma`,`Naam programma`,`duurtijd cyclus`,`Duur sterilisatie`,`Min temp.`,`Max temp`,`Machine_naam`) VALUES ('" + array2Da[0, 1] + "', '" + array2Da[1, 1] + "', '" + array2Da[2, 1] + "', '" + array2Da[3, 1] + "', '" + array2Da[4, 1] + "', '0-Cycle terminé correctement','420','Test vide EN285','1230', '0','1', '0', 'Autoclave 1')";

            command = new MySqlCommand(sql, conn);
            adapter.InsertCommand = new MySqlCommand(sql, conn);
            adapter.InsertCommand.ExecuteNonQuery();

            command.Dispose();

            conn.Close();

        }
    }
}