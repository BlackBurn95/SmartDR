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
using Android.Preferences;

namespace SmartDR2
{
    [Activity(Label = "@string/ApplicationName")]
    public class PatientActivity : Activity
    {
        private Button drug, date, logout, refresh;
        private DBManage db;
        private SqliteDB sqlDB;
        private string id, username;
        private bool f;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.patient_layout);

            drug = FindViewById<Button>(Resource.Id.pat_drug_btn);
            date = FindViewById<Button>(Resource.Id.pat_date_btn);
            logout = FindViewById<Button>(Resource.Id.pat_logout_btn);
            refresh = FindViewById<Button>(Resource.Id.pat_ref_btn);

            db = new DBManage();
            f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "No Connection", ToastLength.Short).Show();
              //  Finish();
            }

            username = Intent.GetStringExtra("username") ?? "";
            id = getID();
            //Toast.MakeText(this, "ID = " + id, ToastLength.Short).Show();
            sqlDB = new SqliteDB(Convert.ToInt32(id));

            update();
            Intent it2 = new Intent(this,typeof(PatientService));
            it2.PutExtra("ID", id);

            StartService(it2);

            drug.Click += delegate
            {
                //update();
                Intent it = new Intent(this, typeof(DrugsActivity));
                it.PutExtra("PID", Convert.ToInt32(id));
                StartActivity(it);
            };

            logout.Click += delegate
            {
                Intent it = new Intent(this, typeof(MainActivity));
                it.PutExtra("logout", "now");
                StartActivity(it);
            };

            refresh.Click += delegate
            {
                update();
            };

            date.Click += delegate
            {
                Intent it = new Intent(this, typeof(DateActivity));
                it.PutExtra("PID", Convert.ToInt32(id));
                StartActivity(it);
            };

        }

        public string getID()
        {
            MySqlDataReader idreader = db.executeQuery("select `id` from `users` where `username` = '" + username + "'");
            string res = "0";
            if (idreader != null && idreader.Read())
            {
                res =  idreader.GetString("id");
            }
            idreader.Close();
            return res;
        }

        public void update()
        {
            if (f)
            {
              /*  MySqlDataReader idreader = db.executeQuery("select `id` from `users` where `username` = '" + username + "'");

                if (idreader == null)
                {

                    return;
                }
                idreader.Read();
                id = idreader.GetString("id");
               * */
                int ID = Convert.ToInt32(id);

              // idreader.Close();

                MySqlDataReader rd = db.executeQuery("select * from `Drugs` where `patient_id` = " + ID + "");

                int ndoses = 0, time = 0, npill = 0;
                string dname = "";

                //if(rd==null)Toast.MakeText(this, "Id = "+ID, ToastLength.Short).Show();
                while (rd != null && rd.Read())
                {

                    int did = rd.GetInt32("drug_id");
                    dname = rd.GetString("drug_name");
                    rd.GetInt32("patient_id");
                    rd.GetInt32("dr_id");
                    ndoses = rd.GetInt32("num_of_doses");
                    time = rd.GetInt32("time");
                    npill = rd.GetInt32("num_of_pills");

                    DrugInfo info = sqlDB.select(dname,ID);
                    if (ndoses > 0)
                    {
                        if(!sqlDB.containsDrug(dname,ID)) sqlDB.insertDrugDoctor(dname, ndoses, time, npill, ID);
                        else sqlDB.updateDrug(dname, ndoses, time, npill, ID);
                        setAlarm(did, dname, ndoses, npill, time);
                    }
                   /* else if (ndoses > 0)
                    {
                        //Toast.MakeText(this, dname + " , " + ndoses, ToastLength.Short).Show();
                        sqlDB.updateDrug(dname, ndoses, time, npill);
                        setAlarm(did, dname, ndoses, npill, time);
                    }*/
                    //else if (ndoses == 0)
                      //  sqlDB.deleteDrug(dname);


                }

                rd.Close();

                rd = db.executeQuery("select * from `dates` where `patient_id` = " + ID + "");

                string drname, loc, t2;
                int aft;

                while (rd != null && rd.Read())
                {

                    int did = rd.GetInt32("dr_id");
                    loc = rd.GetString("location");
                    t2 = rd.GetString("time");

                    int mn = (DateTime.ParseExact(t2, "dd/MM/yyyy HH:mm", null) - DateTime.Now).Minutes;

                    if(!sqlDB.isContainDate(t2)) sqlDB.insertDate(did, loc, t2, Convert.ToInt32(id));
                    
                    if(mn<=1440)
                        setDateAlarm(did, loc, t2, mn);
                }

                rd.Close();
            }
        }

        public void setDateAlarm(int did, string loc, string tt,int tt2)
        {
            AlarmManager man = (AlarmManager)GetSystemService(Context.AlarmService);
            Intent it = new Intent(this, typeof(DatesAlarmReciver));
            it.PutExtra("DID", did);
            it.PutExtra("loc", loc);
            it.PutExtra("time", tt);
            it.PutExtra("PID", Convert.ToInt32(id));

            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(this);

            int rc = pre.GetInt("remid", 0);

            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutInt("remid", rc + 1);
            edit.Apply();

            PendingIntent pit = PendingIntent.GetBroadcast(this, rc, it, 0);
            man.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + tt2 * 60 * 1000 - (24*60*60*1000), pit); 
        }

        public void setAlarm(int num,string dname,int doses,int pill,int time)
        {
            AlarmManager man = (AlarmManager)GetSystemService(Context.AlarmService);
            Intent it = new Intent(this, typeof(AlarmReciver));
            it.PutExtra("PID", Convert.ToInt32(id));
            it.PutExtra("name", dname);
            it.PutExtra("doses", ""+doses);
            it.PutExtra("pill", ""+pill);
            it.PutExtra("time", time);
            it.PutExtra("num", num);

            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(this);
            
            int rc = pre.GetInt("remid", 0);

            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutInt("remid", rc + 1);
            edit.Apply();

            PendingIntent pit = PendingIntent.GetBroadcast(this, rc, it, 0);
            man.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + time * 1000, pit); //time in second
        }
    }
}