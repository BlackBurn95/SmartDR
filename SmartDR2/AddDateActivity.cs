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
    [Activity(Label = "AddDateActivity", ParentActivity = typeof(Doctor))]
    public class AddDateActivity : Activity
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private int id,pid;
        private Button addbtn, cancbtn;
        private EditText year, mon, day, hour, minit, pat_id,loc;
        private string yyyy, MM, dd, HH, mm, location;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.add_date_activity);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            db = new DBManage();
            bool f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "No Connection", ToastLength.Short).Show();
                //  Finish();
            }

            int drID = Intent.GetIntExtra("DRID", 0);

            sqlDB = new SqliteDB(drID);

            addbtn = FindViewById<Button>(Resource.Id.adddate_add_btn);
            cancbtn = FindViewById<Button>(Resource.Id.adddate_canc_btn);

            year = FindViewById<EditText>(Resource.Id.yyyy);
            mon = FindViewById<EditText>(Resource.Id.MM);
            day = FindViewById<EditText>(Resource.Id.dd);
            hour = FindViewById<EditText>(Resource.Id.HH);
            minit = FindViewById<EditText>(Resource.Id.mm);
            pat_id = FindViewById<EditText>(Resource.Id.adddate_pid);
            loc = FindViewById<EditText>(Resource.Id.adddate_location);

            cancbtn.Click += delegate
            {
                Finish();
            };

            addbtn.Click += delegate
            {
                yyyy = year.Text.ToString();
                MM = mon.Text.ToString();
                if (MM.Length == 1) MM = "0" + MM;
                dd = day.Text.ToString();
                if (dd.Length == 1) dd = "0" + dd;
                HH = hour.Text.ToString();
                if (HH.Length == 1) HH = "0" + HH;
                mm = minit.Text.ToString();
                if (mm.Length == 1) mm = "0" + mm;

                pid = Convert.ToInt32(pat_id.Text.ToString());
                location = loc.Text.ToString();
                string time = dd+"/"+MM+"/"+yyyy+" "+HH+":"+mm;
                if (!sqlDB.isContainDate(time))
                {
                    sqlDB.insertDate(drID, location, time, pid);
                    db.exectueDML("insert into `dates` (dr_id,patient_id,location,time) values(" + drID + "," + pid + ",'" + location + "','" + time + "')");
                }
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