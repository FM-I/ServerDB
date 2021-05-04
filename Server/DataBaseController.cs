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

        /// <summary>
        /// Підключення до база даних
        /// </summary>
        SqlConnection connection;
        /// <summary>
        /// Контекст обраної таблиці
        /// </summary>
        DataSet currentData = new DataSet();

        public DataBaseController()
        {
            connection = new SqlConnection(defaultConnection);
            connection.Open();
            Console.WriteLine(GetDataBases());
            connection.Close();
            connection.Dispose();

            Console.WriteLine();
            Console.WriteLine(GetTables("Musics"));
            Console.WriteLine();
            Console.WriteLine(GetTableItems("Groups"));
        }

        /// <summary>
        /// Отримання списку всіх баз даних
        /// </summary>
        /// <returns>Список баз даних у форматі Json</returns>
        public string GetDataBases()
        {
            var dataBases = connection.GetSchema("Databases");
            return GetJson(dataBases, "database_name", "DataBase");
        }

        /// <summary>
        /// Отримання списку таблиць бази даних
        /// </summary>
        /// <param name="dataBase">Назва бази даних</param>
        /// <returns>Список таблиць бази даних у форматі Json</returns>
        public string GetTables(string dataBase)
        {
            connection = new SqlConnection($@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog={dataBase};Integrated Security=True;");
            connection.Open();

            var Tables = connection.GetSchema("Tables");

            return GetJson(Tables, "TABLE_NAME", "Tables");
        }

        /// <summary>
        /// Отримання контексту бази даних
        /// </summary>
        /// <param name="table">Назва таблиці</param>
        /// <returns>Контекст бази даних у форматі Json</returns>
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

        /// <summary>
        /// Отримання списку об'єктів стовбця в таблиці
        /// </summary>
        /// <param name="rows">Лінії таблиці</param>
        /// <param name="col">Назва стовбця</param>
        /// <returns>Список значень в стовбці</returns>
        private List<string> GetItemRow(DataRowCollection rows, DataColumn col)
        {
            List<string> rowItems = new List<string>();
            foreach (DataRow row in rows)
            {
                rowItems.Add(row[col].ToString());
            }
            return rowItems;
        }

        /// <summary>
        /// Отримання Json
        /// </summary>
        /// <param name="data">Об'єкт DataTable</param>
        /// <param name="key">Назва параметру для вибору з DataTable</param>
        /// <param name="jsonKeyName">Назва ключа під яким зберігатиметься список</param>
        /// <returns>Json</returns>
        private string GetJson(DataTable data, string key, string jsonKeyName)
        {
            List<string> dataList = new List<string>();

            foreach (DataRow row in data.Rows)
            {
                dataList.Add(row[key].ToString());
            }

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            dict[jsonKeyName] = dataList;
            return JsonSerializer.Serialize(dict);
        }


        ~DataBaseController()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
