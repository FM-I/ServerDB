using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

using MySql.Data.MySqlClient;

namespace Server
{
    /// <summary>
    /// Клас для підключенн та взаємодії з Базою даних
    /// </summary>
    public class DataBaseController
    {
        /// <summary>
        /// Рядок підключення
        /// </summary>
        MySqlConnectionStringBuilder builder;

        /// <summary>
        /// Підключення до база даних
        /// </summary>
        MySqlConnection connection;
        /// <summary>
        /// Контекст обраної таблиці
        /// </summary>
        DataSet currentData = new DataSet();

        /// <summary>
        /// Створення підключення до бази даних
        /// </summary>
        /// <param name="server">IP або назва серверу</param>
        /// <param name="port">Порт підключення</param>
        /// <param name="userID">Логін користувача</param>
        /// <param name="password">Пароль підключення до бази даних</param>
        /// <param name="database">Назва бази даних</param>
        public DataBaseController(string server, uint port, string userID, string password, string database = "")
        {

            builder = new MySqlConnectionStringBuilder();

            builder.Server = server;
            builder.UserID = userID;
            builder.Password = password;
            builder.Database = database;
            builder.Port = port;

            connection = new MySqlConnection(builder.ConnectionString);
            connection.Open();
            Console.WriteLine(GetDataBases());
            connection.Close();
            connection.Dispose();

            Console.WriteLine();
            Console.WriteLine(GetTables("school"));
            Console.WriteLine();
            var x = JsonSerializer.Deserialize<Dictionary<string, ArrayList>>(GetTableItems("prozoro"));

            foreach (var col in x)
            {
                Console.WriteLine(col.Key);
                foreach (var item in col.Value)
                {
                    Console.WriteLine(item.GetType());
                }
            }

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
            builder.Database = dataBase;
            connection = new MySqlConnection(builder.ConnectionString);
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
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter($"SELECT * FROM {table}", connection);
            dataAdapter.Fill(currentData);
            Dictionary<string, ArrayList> dict = new Dictionary<string, ArrayList>();

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
        private ArrayList GetItemRow(DataRowCollection rows, DataColumn col)
        {
            ArrayList rowItems = new ArrayList();
            foreach (DataRow row in rows)
            {
                rowItems.Add(row[col]);
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
