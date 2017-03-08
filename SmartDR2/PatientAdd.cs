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
    [Activity(Label = "PatientAdd",ParentActivity=typeof(Doctor))]
    public class PatientAdd : Activity
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private EditText nameTxt, idTxt;
        private Button add, cancel;
        private int id;
        private string name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.add_patient_layout);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            nameTxt = FindViewById<EditText>(Resource.Id.patient_name_txt1);
            idTxt = FindViewById<EditText>(Resource.Id.patient_id_txt1);
            add = FindViewById<Button>(Resource.Id.patient_update1);
            cancel = FindViewById<Button>(Resource.Id.patient_cancel1);

            db = new DBManage();
            bool f = db.connect();

            if (!f)
            {
                Finish();
            }

            sqlDB = new SqliteDB(Intent.GetIntExtra("DRID",0));

            add.Click += delegate
            {
                MySqlDataReader rd = db.executeQuery("select * from `users` where id = "+(idTxt.Text.ToString())+" or Name = '"+(nameTxt.Text.ToString())+"'");

                if (rd != null && rd.Read())
                {
                    int id = Convert.ToInt32(rd.GetString("id"));
                    string name = rd.GetString("Name");
                    string email = rd.GetString("email");
                    string mob = rd.GetString("mobile");
                    rd.Close();

                    PatientInfo info = sqlDB.selectPatient(name);
                    if (info==null)
                    {
                        sqlDB.addPatient(id, name, email, mob);

                       

                        rd = db.executeQuery("select * from `Drugs` where patient_id = " + id);

                        while (rd != null && rd.Read())
                        {
                            name = rd.GetString("drug_name");
                            int dos = rd.GetInt32("num_of_doses");
                            int time = rd.GetInt32("time");
                            int pill = rd.GetInt32("num_of_pills");

                            sqlDB.insertDrugDoctor(name, dos, time, pill, id);
                        }

                        rd.Close();
                    }
                }

                Finish();
            };

            cancel.Click += delegate
            {
                Finish();
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}