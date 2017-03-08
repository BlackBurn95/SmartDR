using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;
using MySql.Data.MySqlClient;

namespace SmartDR2
{
    [Activity(Label = "SmartDR2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button login, reg;
        private EditText username_txt, password_txt;
        private bool logged,isDr;
        private ISharedPreferences data;
        private DBManage db;
        private string username, password;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);

            login = FindViewById<Button>(Resource.Id.main_login_btn);
            reg = FindViewById<Button>(Resource.Id.main_register);

            username_txt = FindViewById<EditText>(Resource.Id.main_name_txt);
            password_txt = FindViewById<EditText>(Resource.Id.main_password_txt);
            
            checkStoredData();

            string state = Intent.GetStringExtra("logout") ?? "NOT";
            if (state.Equals("now"))
            {
                logged = false;
                isDr = false;
                username = "";
                password = "";
                storeData();
            }

            if (logged) logIn();

            db = new DBManage();
            bool f = db.connect();

            if (!f)
            {
                Toast.MakeText(this, "Connection Error!", ToastLength.Short).Show();
                login.Enabled = false;
                reg.Enabled = false;
            }

            login.Click += delegate
            {
                username = username_txt.Text.ToString();
                password = password_txt.Text.ToString();

                MySqlDataReader rd = db.executeQuery("select * from users where username = '" + username + "' and password = '" + password + "'");
                if (rd != null && rd.Read())
                {
                    logged = true;
                    Toast.MakeText(this, "LoggedIn", ToastLength.Short).Show();
                    isDr = rd.GetBoolean("is_dr");
                    logIn();
                }
                else Toast.MakeText(this, "Wrong username/password", ToastLength.Short).Show();
            };

            reg.Click += delegate
            {
                register();
            };
        }

        public void logIn()
        {
            storeData();
            Intent it ;

            if (isDr) it = new Intent(this, typeof(Doctor));
            else it = new Intent(this, typeof(PatientActivity));
            
            
            it.PutExtra("username", username);
            StartActivity(it);
        }

        public void register()
        {
            Intent it = new Intent(this, typeof(RegisterActivity));
            StartActivityForResult(it, 1);
        }

        public void storeData()
        {
            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutBoolean("logged", logged);
            edit.PutString("username", username);
            edit.PutString("password", password);
            edit.PutBoolean("isdr", isDr);
            edit.Apply();
        }

        public void checkStoredData()
        {
            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(this);
            logged = pre.GetBoolean("logged", false);
            username = pre.GetString("username", "");
            password = pre.GetString("password", "");
            isDr = pre.GetBoolean("isdr", false);
        }

        protected override void OnStop()
        {
            base.OnStop();
            storeData();
            db.disconnect();
        }

        protected override void OnActivityResult(int rCode, Result r, Intent data)
        {
            base.OnActivityResult(rCode, r, data);

            if (rCode == 1 && r == Result.Ok)
            {
                username = data.GetStringExtra("user");
                password = data.GetStringExtra("pass");
                isDr = data.GetBooleanExtra("isdr",false);
                logged = true;
                logIn();
            }
        }
    }
}

