using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.IO
{
    public class StorageIO
    {
        private readonly string storageName = "\\data.db";
        private readonly string storagePath;
        private readonly string connectionString;

        public StorageIO()
        {
            storagePath = Directory.GetCurrentDirectory() + storageName;
            connectionString = "Data Source =" + storagePath + ";";
            if (!File.Exists(storagePath))
            {
                CreateDB();
            }
        }

        private void CreateDB()
        {
            using SqliteConnection connection = new(connectionString);
            connection.Open();
            using SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = "CREATE TABLE EVTCFile " +
                "(FullPath      TEXT NOT NULL," +
                "Name           TEXT NOT NULL," +
                "CreationDate   INTEGER NOT NULL," +
                "UserName       TEXT," +
                "CharcterName   TEXT," +
                "Length         INTEGER," +
                "CharcterClass  INTEGER," +
                "CharcterSpec   INTEGER," +
                "Encounter      INTEGER," +
                "UploadUrl      TEXT," +
                "Success        INTEGER," +
                "PRIMARY KEY(FullPath));";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public ConcurrentBag<ListItem> GetRecords()
        {
            using SqliteConnection connection = new(connectionString);
            connection.Open();
            using SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM EVTCFile;";
            SqliteDataReader reader = command.ExecuteReader();
            ConcurrentBag<ListItem> x = new();

            while (reader.Read())
            {
                string url;
                if (reader.IsDBNull("UploadUrl"))
                    url = "";
                else
                    url = (string)reader.GetValue("UploadUrl");

                x.Add(new((string)reader.GetValue("FullPath"),
                    (string)reader.GetValue("Name"),
                    new((long)reader.GetValue("CreationDate")),
                    (string)reader.GetValue("UserName"),
                    (string)reader.GetValue("CharcterName"),
                    new((long)reader.GetValue("Length")),
                    (Profession)(long)reader.GetValue("CharcterClass"),
                    (Specialization)(long)reader.GetValue("CharcterSpec"),
                    (Encounter)(long)reader.GetValue("Encounter"),
                    url
                    ));
            }

            connection.Close();
            return x;
        }

        public void AddRecords(ConcurrentBag<ListItem> records)
        {
            using SqliteConnection connection = new(connectionString);
            connection.Open();
            foreach (ListItem item in records)
            {
                // Check if the file is already in the database, if yes, skip
                using SqliteCommand checkCommand = new();
                checkCommand.Connection = connection;
                checkCommand.CommandText = "SELECT * FROM EVTCFile WHERE FullPath = @FullPath;";
                checkCommand.Parameters.Add("@FullPath", SqliteType.Text).Value = item.FullPath;
                SqliteDataReader reader = checkCommand.ExecuteReader();
                if (reader.HasRows) continue;

                using SqliteCommand command = new();
                command.Connection = connection;
                command.CommandText = "INSERT INTO EVTCFile (" +
                    "FullPath,Name,CreationDate,UserName,CharcterName,Length,CharcterClass,CharcterSpec,Encounter,Success" +
                    ") VALUES (" +
                    "@FullPath,@Name,@CreationDate,@UserName,@CharcterName,@Length,@CharcterClass,@CharcterSpec,@Encounter,@Success);";
                command.Parameters.Add("@FullPath", SqliteType.Text).Value = item.FullPath;
                command.Parameters.Add("@Name", SqliteType.Text).Value = item.Name;
                command.Parameters.Add("@CreationDate", SqliteType.Integer).Value = item.CreationDate.Ticks;
                command.Parameters.Add("@UserName", SqliteType.Text).Value = item.UserName;
                command.Parameters.Add("@CharcterName", SqliteType.Text).Value = item.CharcterName;
                command.Parameters.Add("@Length", SqliteType.Integer).Value = item.Length.Ticks;
                command.Parameters.Add("@CharcterClass", SqliteType.Integer).Value = (int)item.CharcterClass;
                command.Parameters.Add("@CharcterSpec", SqliteType.Integer).Value = (int)item.CharcterSpec;
                command.Parameters.Add("@Encounter", SqliteType.Integer).Value = (int)item.Encounter;
                command.Parameters.Add("@Success", SqliteType.Integer).Value = 0;
                _ = command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public void UpdateRecordsURL(List<ListItem> newValues)
        {
            using SqliteConnection connection = new(connectionString);
            connection.Open();
            foreach (ListItem item in newValues)
            {
                using SqliteCommand command = new();
                command.Connection = connection;
                command.CommandText = "UPDATE EVTCFile SET " +
                    "UploadUrl = @UploadUrl " +
                    "WHERE " +
                    "FullPath = @FullPath";
                command.Parameters.Add("@UploadUrl", SqliteType.Text).Value = item.UploadUrl;
                command.Parameters.Add("@FullPath", SqliteType.Text).Value = item.FullPath;
                _ = command.ExecuteNonQuery();
            }
        }

        public void WipeDB()
        {
            if (File.Exists(storagePath))
            {
                using SqliteConnection connection = new(connectionString);
                connection.Open();
                using SqliteCommand command = new();
                command.Connection = connection;
                command.CommandText = "DROP TABLE EVTCFile";
                command.ExecuteNonQuery();
                connection.Close();
                CreateDB();
            }
        }
    }
}
