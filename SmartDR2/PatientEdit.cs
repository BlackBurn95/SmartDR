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
    [Activity(Label = "PatientEdit",ParentActivity=typeof(Doctor))]
    public class PatientEdit : Activity
    {
        private DBManage db;
        private SqliteDB sqlDB;
        private int id;
        private EditText pidTxt, drugNameTxt, dosesTxt, timeTxt, pillText;
        private Switch sw;
        private Button updateBtn,cancelBtn;
        private string drg_name;
        private int doses, time, pills;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.patient_edit_layout);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            db = new DBManage();
            bool f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "No Connection", ToastLength.Short).Show();
                //  Finish();
            }

            pidTxt = FindViewById<EditText>(Resource.Id.patient_pid_txt);
            drugNameTxt = FindViewById<EditText>(Resource.Id.patient_drugid_txt);
            dosesTxt = FindViewById<EditText>(Resource.Id.patient_dosesnum_txt);
            timeTxt = FindViewById<EditText>(Resource.Id.patient_time_txt);
            pillText = FindViewById<EditText>(Resource.Id.patient_pill_txt);
            updateBtn = FindViewById<Button>(Resource.Id.patient_update);
            cancelBtn = FindViewById<Button>(Resource.Id.patient_cancel);
            sw = FindViewById<Switch>(Resource.Id.switch1);

            bool isnew = false;

            sw.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                isnew = e.IsChecked;
            };
            int drID = Intent.GetIntExtra("DRID",0);

            sqlDB = new SqliteDB(drID);

            updateBtn.Click += delegate
            {
                id = Convert.ToInt32(pidTxt.Text.ToString());
                drg_name = drugNameTxt.Text.ToString();
                doses = Convert.ToInt32(dosesTxt.Text.ToString());
                time = Convert.ToInt32(timeTxt.Text.ToString());
                pills = Convert.ToInt32(pillText.Text.ToString());

              /*  if (!isnew)
                {
                    db.exectueDML("update `Drugs` set num_of_doses = " + doses + ", time = " + time + ", num_of_pills = " + pills + " where patient_id = " + id + " and drug_name = '" + drg_name + "'");
                    sqlDB.updateDrugDoctor(drg_name, doses, time, pills, id);
                }
                else
                {
                */
                    if (sqlDB.containsDrug(drg_name,id))
                    {
                        Toast.MakeText(this, "UPDATE", ToastLength.Short).Show();
                        sqlDB.updateDrugDoctor(drg_name, doses, time, pills, id);
                    }
                    else
                    {
                        Toast.MakeText(this, "INSERT", ToastLength.Short).Show();
                        sqlDB.insertDrugDoctor(drg_name, doses, time, pills, id);
                    }
                    MySqlDataReader rd = db.executeQuery("select * from `Drugs` where `drug_name` = '" + drg_name + "' and patient_id = "+id);
                    if (rd == null || !rd.Read())
                    {
                        if(rd!=null) rd.Close();
                        bool ff = db.exectueDML("insert into `Drugs` (drug_name,patient_id,dr_id,num_of_doses,time,num_of_pills) values('" + drg_name + "'," + id + "," + drID + "," + doses + ", " + time + "," + pills + ")");
                        Toast.MakeText(this, "Patient Data Inserted : "+(ff ? "SUCCESS" : "FAILD"), ToastLength.Short).Show();
                    }
                    else
                    {
                        rd.Close();
                        Toast.MakeText(this, "Patient Data Updated", ToastLength.Short).Show();
                        db.exectueDML("update `Drugs` set num_of_doses = " + doses + ", time = " + time + ", num_of_pills = " + pills + " where patient_id = " + id + " and drug_name = '" + drg_name + "'");
                    }
            //    }

               
                Finish();
            };

            cancelBtn.Click += delegate
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