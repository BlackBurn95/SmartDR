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
    [Activity(Label = "@string/ApplicationName")]
    public class RegisterActivity : Activity
    {
        private EditText username, id, email, name, mob, pass1wd, pass2wd;
        private Button reg_btn, canc_btn;
        private Switch isdr_sw;
        private DBManage db;

        private string username_txt, name_txt, id_txt, email_txt, mob_txt, pass1wd_txt, pass2wd_txt;
        private bool isDr;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_layout);

            username = FindViewById<EditText>(Resource.Id.reg_username_txt);
            id = FindViewById<EditText>(Resource.Id.reg_id_txt);
            name = FindViewById<EditText>(Resource.Id.reg_name_txt);
            email = FindViewById<EditText>(Resource.Id.reg_email_txt);
            mob = FindViewById<EditText>(Resource.Id.reg_mobile_txt);
            pass1wd = FindViewById<EditText>(Resource.Id.reg_password1_txt);
            pass2wd = FindViewById<EditText>(Resource.Id.reg_password2_txt);

            reg_btn = FindViewById<Button>(Resource.Id.reg_register_btn);
            canc_btn = FindViewById<Button>(Resource.Id.reg_cancel_btn);

            isdr_sw = FindViewById<Switch>(Resource.Id.reg_isdr_sw);

            db = new DBManage();
            bool f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "Connection Error!", ToastLength.Short).Show();
                exit_activity();
            }

            canc_btn.Click += delegate
            {
                exit_activity();
            };

            isdr_sw.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                isDr = e.IsChecked;
            };

            reg_btn.Click += delegate
            {
                username_txt = username.Text.ToString();
                name_txt = name.Text.ToString();
                id_txt = id.Text.ToString();
                pass1wd_txt = pass1wd.Text.ToString();
                pass2wd_txt = pass2wd.Text.ToString();
                email_txt = email.Text.ToString();
                mob_txt = mob.Text.ToString();

                int Id = Convert.ToInt32(id_txt);

                MySqlDataReader rd = db.executeQuery("select * from `users` where id = '" + Id + "' or username = '" + username_txt + "' or email = '" + email_txt);

                if (!pass1wd_txt.Equals(pass2wd_txt))
                    Toast.MakeText(this, "Password Don't Match", ToastLength.Short).Show();
                else if (rd != null)
                    Toast.MakeText(this, "Wrong Information", ToastLength.Short).Show();
                else
                {
                    bool f2 = db.exectueDML("INSERT INTO `sql6157162`.`users` (`id`, `is_dr`, `Name`, `username`, `email`, `mobile`, `password`) VALUES ('" + Id + "','"+(isDr ? 1 : 0)+ "','" + name_txt + "' , '" + username_txt + "' , '" + email_txt + "' , '" + mob_txt + "' , '" + pass1wd_txt + "')");
                    if (f2)
                    {
                        Toast.MakeText(this, "New User Registered", ToastLength.Short).Show();
                        Intent it = new Intent(this, typeof(MainActivity));
                        it.PutExtra("user", username_txt);
                        it.PutExtra("pass", pass1wd_txt);
                        it.PutExtra("isdr", isDr);
                        SetResult(Result.Ok, it);
                        db.disconnect();
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Something Went Wrong, try again later", ToastLength.Short).Show();
                        exit_activity();
                    }
                }
            };
        }

        public void exit_activity()
        {
            Intent it = new Intent(this, typeof(MainActivity));
            SetResult(Result.Canceled, it);
            db.disconnect();
            Finish();
        }
    }
}