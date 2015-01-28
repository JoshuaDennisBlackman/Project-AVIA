using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Data;

namespace NetworkServer
{
    class DatabaseInterface
    {
        private MySqlConnection connection;

        public DatabaseInterface(string credentialsPath)
        {
            //w(ConsoleColor.Cyan, "Testing database connection...");
            string[] credentials = new string[0];
            try { credentials = File.ReadAllLines(credentialsPath); }
            catch (Exception ex) { w(ConsoleColor.Red, "Invalid database credentials. Press any key..."); ; Console.ReadLine(); Environment.Exit(0); }

            string connectionString;
            connectionString = "SERVER=" + credentials[0] + ";" + "DATABASE=" +
            credentials[1] + ";" + "UID=" + credentials[2] + ";" + "PASSWORD=" + credentials[3] + ";";
            connection = new MySqlConnection(connectionString);
            OpenConnection();
            CloseConnection();
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                //w(ConsoleColor.Green, "Database connection established.");
                return true;
            }
            catch (MySqlException ex)
            {
                w(ConsoleColor.Red, "Database connection could not be established.");
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                w(ConsoleColor.Red, "Database connection could not be established.");
                return false;
            }
        }

        public DataTable ExecuteQuery(string query)
        {
           if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(dataReader);
                dataReader.Close();
                this.CloseConnection();
                if (data.Rows.Count != 0) { return data; } else { return null; }
            }
            else 
            { 
               //throw new NAIMException("Unable to connect to central database."); 
                w(ConsoleColor.Red, "Could not execute query.");
                return null;
            }
        }

        public bool BookMeeting(string details)
        {
            
            try
            {
                DataTable userCheck = ExecuteQuery("INSERT INTO meetings (details) VALUES ('" + details + "');");
                w(ConsoleColor.Green, "Booking inserted succesfully.");
                return true;
            }
            catch(MySqlException ex)
            {
                w(ConsoleColor.Red, "Booking insertion failed.");
                return false; 
            }
        }

        //private string UidLookup(string username)
        //{
        //    //DataTable userCheck = ExecuteQuery("SELECT * FROM " + "users" + " WHERE " + "(" + "username" + "='" + username + "');");
        //    //if (userCheck != null)
        //    //{
        //    //    return userCheck.Rows[0].Field<string>(0);
        //    //}
        //    //else { return null; }
        //}

        //public bool Authorise(string username, string password)
        //{
        //    //DataTable authCheck = ExecuteQuery("SELECT * FROM " + "users" + " WHERE " + "(" + "username" + "='" + username + "' AND password='" + password + "');");
        //    //if (authCheck != null)
        //    //{
        //    //    return true;
        //    //}
        //    //else { //throw new NAIMException("Unable to authorise user."); 
        //    //}
        //}

        //public bool RegisterUser(string username, string password, string email)
        //{
        //    //if (Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$"))
        //    //{
        //    //    DataTable usernameCheck = ExecuteQuery("SELECT * FROM " + "users" + " WHERE " + "username" + "='" + username + "';");
        //    //    if (usernameCheck == null)
        //    //    {
        //    //        ExecuteQuery("INSERT INTO users (username, password, email) VALUES ('" + username + "', '" + password + "', '" + email + "');");
        //    //        w(ConsoleColor.Green, "User " + username + " has registered succesfully.");
        //    //        return true;
        //    //    }
        //    //    else { //throw new NAIMException("Registration failed, username already exists."); 
        //    //    }
        //    //}
        //    //else { //throw new NAIMException("Invalid email."); 
        //    //}
        //}

        //public bool UnregisterUser(string username, string password)
        //{
        //    //if (Authorise(username, password) != false)
        //    //{
        //    //    ExecuteQuery("DELETE FROM " + "users" + " WHERE " + "username" + "='" + username + "';");
        //    //    Program.Log(ConsoleColor.Green, "User " + username + " has unregistered succesfully.");
        //    //    return true;
        //    //}
        //    //else { //throw new NAIMException("Unregistration failed, unable to authorise user."); 
        //    //}
        //} 

        private void w(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "] >> " + text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
