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
using SQLite;
using System.IO;
using MySql.Data.MySqlClient;

namespace SmartDR2
{
    class SqliteDB
    {
        //database path
        string dbPath;

        public SqliteDB(int user)
        {
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Data12"+user+".db3");
            //Creating database, if it doesn't already exist 
            if (!File.Exists(dbPath))
            {
                var db = new SQLiteConnection(dbPath);
                db.CreateTable<Drugs>();
                db.CreateTable<Patient>();
                db.CreateTable<Dates>();
            }
            //if (db.Table<Departments>().Count() == 0)
        }

        public void insertDrug(string name,int d,int t,int pill,int pid)
        {
            var db = new SQLiteConnection(dbPath);
            var drug = new Drugs();
            drug.DrugName = name;
            drug.NumDoses = d;
            drug.Time = t;
            drug.NumPill = pill;
            drug.PatientId = pid;
            db.Insert(drug);
        }

        public void insertDrugDoctor(string name, int d, int t, int pill,int pid)
        {
            var db = new SQLiteConnection(dbPath);
            var drug = new Drugs();
            drug.DrugName = name;
            drug.NumDoses = d;
            drug.Time = t;
            drug.NumPill = pill;
            drug.PatientId = pid;
            db.Insert(drug);
        }

        public void updateDrug(string drug, int d, int t, int p,int id)
        {
            using (var con = new SQLiteConnection(dbPath))
            {
                try
                {
                    con.Query<Drugs>("update Drugs set DrugName=?,NumDoses=?,Time=?,NumPill=? where DrugName=? and PatientId=?", drug, d, t, p, drug,id);
                }
                catch (Exception e) { }
            }
        }

        public void updateDrugDoctor(string drug, int d, int t, int p,int pid)
        {
            using (var con = new SQLiteConnection(dbPath))
            {
                try
                {
                    con.Query<Drugs>("update Drugs set DrugName=?,NumDoses=?,Time=?,NumPill=? where DrugName=? and PatientId=?", drug, d, t, p, drug, pid);
                }
                catch (Exception e) { }
            }
        }

        public void deleteDrug(string name,int id)
        {
            using (var con = new SQLiteConnection(dbPath))
            {
                try
                {
                    con.Query<Drugs>("delete from Drugs where DrugName=? and PatientId", name, id);
                }
                catch (Exception e) { }
            }
        }

        public DrugInfo select(string name,int id)
        {
            var db = new SQLiteConnection(dbPath);

            var d = db.Query<Drugs>("SELECT * FROM Drugs WHERE DrugName = ? and PatientId = ?", name, id);

            foreach (var s in d)
                return new DrugInfo(s.DrugName, s.NumDoses, s.Time, s.NumPill);

            return null;
        }

        public List<DrugInfo> selectDrugByID(int id)
        {
            var db = new SQLiteConnection(dbPath);

            List<DrugInfo> data = new List<DrugInfo>();

            var d = db.Query<Drugs>("SELECT * FROM Drugs WHERE PatientId = ?", id);

            foreach (var s in d)
                data.Add(new DrugInfo(s.DrugName, s.NumDoses, s.Time, s.NumPill));
                //else deleteDrug(s.DrugName);

            return data;
        }

        public int getNumDoses(string name,int id)
        {
            DrugInfo d = select(name, id);
            if (d == null)
                return 0;
            return d.numDoses;
        }

        public bool containsPatient(string name)
        {
            return selectPatient(name) != null;
        }

        public bool containsDrug(string name,int id)
        {
            DrugInfo d = select(name,id);
            return d != null;
        }

        public void addPatient(int id, string name, string email, string mob)
        {
            var db = new SQLiteConnection(dbPath);
            var p = new Patient();
            p.PatientId = id;
            p.Name = name;
            p.Email = email;
            p.Mobile = mob;
            db.Insert(p);
        }

        public PatientInfo selectPatient(string name)
        {
            var db = new SQLiteConnection(dbPath);

            var d = db.Query<Patient>("SELECT * FROM Patient WHERE Name = ?", name);

            foreach (var s in d)
                return new PatientInfo(s.PatientId, s.Name, s.Email, s.Mobile);

            return null;
        }

        public List<PatientInfo> selectAllPatient()
        {
            List<PatientInfo> data = new List<PatientInfo>();

            var db = new SQLiteConnection(dbPath);

            var table = db.Table<Patient>();
            foreach (var s in table)
                data.Add(new PatientInfo(s.PatientId, s.Name, s.Email, s.Mobile));

            return data;
        }

        public List<DateInfo> selectDatesByPId(int pid)
        {
            List<DateInfo> data = new List<DateInfo>();

            var db = new SQLiteConnection(dbPath);

            var d = db.Query<Dates>("SELECT * FROM Dates WHERE PatientId = ?", pid);

            foreach (var s in d)
                data.Add(new DateInfo(s.DrName,s.Location,s.Time,pid));

            return data;
        }

        public List<DateInfo> selectDates(int pid)
        {
            List<DateInfo> data = new List<DateInfo>();

            var db = new SQLiteConnection(dbPath);

            var d = db.Query<Dates>("SELECT * FROM Dates");

            foreach (var s in d)
                data.Add(new DateInfo(s.DrName, s.Location, s.Time, s.PatientId));

            return data;
        }

        public void insertDate(int did, string loc, string tt,int pid)
        {
            var db = new SQLiteConnection(dbPath);
            var d = new Dates();

            string dname = "dr";
            DBManage db2 = new DBManage();
            if (db2.connect())
            {
                MySqlDataReader rd = db2.executeQuery("select `Name` from `users` where `id` = " + did + "");
                if(rd.Read())
                    dname = rd.GetString("Name");
                rd.Close();
            }
            db2.disconnect();

            d.DrName = dname;
            d.Location = loc;
            d.Time = tt;
            d.PatientId = pid;

            db.Insert(d);
        }

        public bool isContainDate(string time)
        {
            var db = new SQLiteConnection(dbPath);

            var d = db.Query<Drugs>("SELECT * FROM Dates WHERE Time = ?", time);

            foreach (var s in d)
                return true;

            return false;
        }

        [Table("Drugs")]
        public class Drugs
        {
            [PrimaryKey, AutoIncrement, Column("id")]
            public int Id { get; set; }
            [MaxLength(10)]
            public string DrugName { get; set; }

            public int NumDoses { get; set; }

            public int Time { get; set; }

            public int NumPill { get; set; }

            public int PatientId { get; set; }
        }

        [Table("Dates")]
        public class Dates
        {
            [PrimaryKey, AutoIncrement, Column("id")]
            public int Id { get; set; }
            [MaxLength(10)]
            public string DrName { get; set; }
            [MaxLength(10)]
            public string Location { get; set; }
            [MaxLength(10)]
            public string Time { get; set; }

            public int PatientId { get; set; }
        }

        [Table("Patient")]
        public class Patient
        {
            [PrimaryKey, AutoIncrement, Column("id")]
            public int Id { get; set; }
            public int PatientId { get; set; }
            [MaxLength(10)]
            public string Name { get; set; }
            [MaxLength(10)]
            public string Email { get; set; }
            [MaxLength(10)]
            public string Mobile { get; set; }
        }
    }


}