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
    [Activity(Label = "Doctor")]
    public class Doctor : Activity
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private string username, id;
        private Button viewBtn, addBtn, logoutBtn, editBtn,viewDate, addDate;
        private bool f;
        private int drID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.doctor_layout);

            db = new DBManage();
            f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "No Connection", ToastLength.Short).Show();
                //  Finish();
            }

            username = Intent.GetStringExtra("username") ?? "";

            id = getID();

            sqlDB = new SqliteDB(Convert.ToInt32(id));

            update();

            Intent it2 = new Intent(this, typeof(DoctorService));
            it2.PutExtra("ID", id);

            StartService(it2);

            viewBtn = FindViewById<Button>(Resource.Id.doc_view_btn);
            addBtn = FindViewById<Button>(Resource.Id.doc_add_btn);
            logoutBtn = FindViewById<Button>(Resource.Id.doc_logout_btn);
            editBtn = FindViewById<Button>(Resource.Id.doc_edit_btn);
            viewDate = FindViewById<Button>(Resource.Id.doc_viewdate_btn);
            addDate = FindViewById<Button>(Resource.Id.doc_adddate_btn);

            viewBtn.Click += delegate
            {
                Intent it = new Intent(this,typeof(PatientView));
                it.PutExtra("DRID", drID);
                StartActivity(it);
            };

            addBtn.Click += delegate
            {
                Intent it = new Intent(this, typeof(PatientAdd));
                it.PutExtra("DRID", drID);
                StartActivity(it);
            };

            editBtn.Click += delegate
            {
                Intent it = new Intent(this, typeof(PatientEdit));
                it.PutExtra("DRID", drID);
                StartActivity(it);
            };

            addDate.Click += delegate
            {
                Intent it = new Intent(this, typeof(AddDateActivity));
                it.PutExtra("DRID", drID);
                StartActivity(it);
            };

            viewDate.Click += delegate
            {
                Intent it = new Intent(this, typeof(DrDataActivity));
                it.PutExtra("DRID", drID);
                StartActivity(it);
            };

            logoutBtn.Click += delegate
            {
                Intent it = new Intent(this, typeof(MainActivity));
                it.PutExtra("logout", "now");
                StartActivity(it);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            update();
        }
        protected override void OnRestart()
        {
            base.OnRestart();
            update();
        }

        public string getID()
        {
            MySqlDataReader idreader = db.executeQuery("select `id` from `users` where `username` = '" + username + "'");
            string res = "0";
            if (idreader != null && idreader.Read())
            {
                res = idreader.GetString("id");
            }
            idreader.Close();
            return res;
        }

        public void update()
        {
            if (f)
            {
                MySqlDataReader idreader = db.executeQuery("select `id` from `users` where `username` = '" + username + "'");

                if (idreader == null)
                {
                    return;
                }
                idreader.Read();
                id = idreader.GetString("id");
                drID = Convert.ToInt32(id);

                idreader.Close();

                MySqlDataReader rd = db.executeQuery("select * from `Drugs` where `dr_id` = " + drID + "");

                int ndoses = 0, time = 0, npill = 0;
                string dname = "";

                List<int> val = new List<int>();

                //if(rd==null)Toast.MakeText(this, "Id = "+ID, ToastLength.Short).Show();
                while (rd != null && rd.Read())
                {

                    rd.GetInt32("drug_id");
                    dname = rd.GetString("drug_name");
                    int pid = rd.GetInt32("patient_id");
                    rd.GetInt32("dr_id");
                    ndoses = rd.GetInt32("num_of_doses");
                    time = rd.GetInt32("time");
                    npill = rd.GetInt32("num_of_pills");

                   // DrugInfo info = sqlDB.select(dname,pid);
                    if (!sqlDB.containsDrug(dname,pid)) sqlDB.insertDrug(dname, ndoses, time, npill, pid);
                    else sqlDB.updateDrug(dname, ndoses, time, npill, pid);

                    if(!val.Contains(pid))
                        val.Add(pid);
                }

                rd.Close();

                foreach(var s in val) {
                    rd = db.executeQuery("select * from `users` where id = " + s);

                    while (rd != null && rd.Read())
                    {
                        int pid = rd.GetInt32("id");
                        rd.GetBoolean("is_dr");
                        string name = rd.GetString("Name");
                        rd.GetString("username");
                        string email = rd.GetString("email");
                        string mob = rd.GetString("mobile");
                        rd.GetString("password");

                        if(!sqlDB.containsPatient(name)) sqlDB.addPatient(pid, name, email, mob);
                    }

                    rd.Close();
                }
            }
        }
    }
}