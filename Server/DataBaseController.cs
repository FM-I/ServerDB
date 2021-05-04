using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace Server
{
    /// <summary>
    /// Клас для підключенн та взаємодії з Базою даних
    /// </summary>
    public class DataBaseController
    {
        private const string defaultConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
        private const string connectioString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Musics;Integrated Security=True";
        SqlConnection connection;
        DataSet currentData = new DataSet();

        public DataBaseController()
        {
            connection = new SqlConnection(defaultConnection);
            connection.Open();
            Console.WriteLine(GetTables("Musics"));
            GetTableItems("Groups");
        }

        public string GetDataBases()
        {
            var dataBases = connection.GetSchema("Databases");

            List<string> dataBasesList = new List<string>();

            foreach (DataRow row in dataBases.Rows)
            {
                dataBasesList.Add(row["database_name"].ToString());
            }

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            dict["DataBase"] = dataBasesList;
            return JsonSerializer.Serialize(dict);
        }

        public string GetTables(string dataBase)
        {
            connection.Close();
            connection.Dispose();

            connection = new SqlConnection($@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog={dataBase};Integrated Security=True;");
            connection.Open();

            var Tables = connection.GetSchema("Tables");

            List<string> tables = new List<string>();

            foreach (DataRow row in Tables.Rows)
            {
                tables.Add(row["TABLE_NAME"].ToString());
            }

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            dict["Tables"] = tables;
            return JsonSerializer.Serialize(dict);
        }


        public string GetTableItems(string table)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter($"SELECT * FROM {table}", connection);
            dataAdapter.Fill(currentData);
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();

            foreach (DataColumn col in currentData.Tables[0].Columns)
            {
                dict[col.ColumnName] = GetItemRow(currentData.Tables[0].Rows, col);
            }

            return JsonSerializer.Serialize(dict);
        }

        private List<string> GetItemRow(DataRowCollection rows, DataColumn col)
        {
            List<string> rowItems = new List<string>();
            foreach (DataRow row in rows)
            {
                rowItems.Add(row[col].ToString());
            }
            return rowItems;
        }

        ~DataBaseController()
        {
            connection.Close();
            connection.Dispose();
        }

        private static void ShowDataTable(DataTable table, Int32 length)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-" + length + "}", col.ColumnName);
            }
            Console.WriteLine();

            List<string> tables = new List<string>();

            foreach (DataRow row in table.Rows)
            {

                tables.Add(row["database_name"].ToString());

                //foreach (DataColumn col in table.Columns)
                //{
                //    if (col.DataType.Equals(typeof(DateTime)))
                //        Console.Write("{0,-" + length + ":d}", row[col]);
                //    else if (col.DataType.Equals(typeof(Decimal)))
                //        Console.Write("{0,-" + length + ":C}", row[col]);
                //    else
                //        Console.Write("{0,-" + length + "}", row[col]);
                //}
            }


            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            dict["Tables"] = tables;
            string t = JsonSerializer.Serialize(dict);

            Console.WriteLine(t);

            var dt = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(t);

            
        }

    }
}
