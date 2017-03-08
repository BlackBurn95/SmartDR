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

    [BroadcastReceiver(Exported = true, Label = "Doctor Reciver")]
    [IntentFilter(new String[] { "com.blackburn95.doctor_service" })]
    class DoctorBroadcast : Android.Content.BroadcastReceiver
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private bool f;
        private string id;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "com.blackburn95.doctor_service")
            {
                db = new DBManage();
                f = db.connect();
                id = intent.GetStringExtra("ID");
                sqlDB = new SqliteDB(Convert.ToInt32(id));

                update(context);

                db.disconnect();
            }
        }

        public void update(Context con)
        {
            if (f)
            {
                int drID = Convert.ToInt32(id);

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
                    if (!sqlDB.containsDrug(dname, pid)) sqlDB.insertDrug(dname, ndoses, time, npill, pid);
                    else sqlDB.updateDrug(dname, ndoses, time, npill, pid);

                    if (!val.Contains(pid))
                        val.Add(pid);
                }

                rd.Close();

                foreach (var s in val)
                {
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

                        if (!sqlDB.containsPatient(name)) sqlDB.addPatient(pid, name, email, mob);
                    }

                    rd.Close();
                }
            }
        }
    }
}