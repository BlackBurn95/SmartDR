using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MySql.Data.MySqlClient;

namespace SmartDR2
{
    class DBManage
    {
        //public const string link = "server=sql6.freemysqlhosting.net;database=sql6157162;uid=sql6157162;pwd=8J51dwFUGC;";
        public const string link = "server=104.198.191.77;database=smartdr;uid=root;pwd=vus4axvh;";
        public MySqlConnection conn;
        private bool isOpen;

        public DBManage()
        {
            conn = new MySqlConnection(link);
            isOpen = false;
        }

        public bool connect()
        {
            try
            {
                conn.Open();
                isOpen = true;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool disconnect()
        {
            try
            {
                conn.Close();
                isOpen = false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool isConnected()
        {
            return isOpen;
        }

        public bool exectueDML(string comm)
        {
            if (!isOpen)
                return false;

            try
            {
                MySqlCommand cmd = new MySqlCommand(comm, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public MySqlDataReader executeQuery(string q)
        {
            if (!isOpen)
            {
                isOpen = connect();
                if(!isOpen)
                return null;
            }

            try
            {
                MySqlCommand cmd = new MySqlCommand(q, conn);
                MySqlDataReader data = cmd.ExecuteReader();
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error = " + e);
                return null;
            }
        }
    }
}