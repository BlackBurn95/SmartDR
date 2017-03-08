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
    [BroadcastReceiver(Exported=true, Label="Patient Reciver")]
    [IntentFilter(new String[] { "com.blackburn95.patient_service" })]
    public class PatientBoradcast : Android.Content.BroadcastReceiver
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private bool f;
        private string id;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "com.blackburn95.patient_service")
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

                    DrugInfo info = sqlDB.select(dname, ID);
                    if (ndoses > 0)
                    {
                        if (!sqlDB.containsDrug(dname, ID)) sqlDB.insertDrugDoctor(dname, ndoses, time, npill, ID);
                        else sqlDB.updateDrug(dname, ndoses, time, npill, ID);
                        setAlarm(did, dname, ndoses, npill, time, con);
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

                    if (!sqlDB.isContainDate(t2)) sqlDB.insertDate(did, loc, t2, Convert.ToInt32(id));

                    if (mn <= 1440)
                        setDateAlarm(did, loc, t2, mn, con);
                }

                rd.Close();
            }
        }

        public void setDateAlarm(int did, string loc, string tt, int tt2,Context con)
        {
            AlarmManager man = (AlarmManager)con.GetSystemService(Context.AlarmService);
            Intent it = new Intent(con, typeof(DatesAlarmReciver));
            it.PutExtra("DID", did);
            it.PutExtra("loc", loc);
            it.PutExtra("time", tt);
            it.PutExtra("PID", Convert.ToInt32(id));

            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(con);

            int rc = pre.GetInt("remid", 0);

            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutInt("remid", rc + 1);
            edit.Apply();

            PendingIntent pit = PendingIntent.GetBroadcast(con, rc, it, 0);
            man.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + tt2 * 60 * 1000 - (24 * 60 * 60 * 1000), pit);
        }

        public void setAlarm(int num, string dname, int doses, int pill, int time,Context con)
        {
            AlarmManager man = (AlarmManager)con.GetSystemService(Context.AlarmService);
            Intent it = new Intent(con, typeof(AlarmReciver));
            it.PutExtra("PID", Convert.ToInt32(id));
            it.PutExtra("name", dname);
            it.PutExtra("doses", "" + doses);
            it.PutExtra("pill", "" + pill);
            it.PutExtra("time", time);
            it.PutExtra("num", num);

            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(con);

            int rc = pre.GetInt("remid", 0);

            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutInt("remid", rc + 1);
            edit.Apply();

            PendingIntent pit = PendingIntent.GetBroadcast(con, rc, it, 0);
            man.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + time * 1000, pit); //time in second
        }
    }
}