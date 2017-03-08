
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
using Android.Media;
using Android.Preferences;
using MySql.Data.MySqlClient;

namespace SmartDR2
{
    [BroadcastReceiver(Enabled = true)]
    public class DatesAlarmReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent it)
        {
            int did = it.GetIntExtra("DID", 0);
            string loc = it.GetStringExtra("loc");
            string time = it.GetStringExtra("time");

            DBManage db = new DBManage();
            int pid = it.GetIntExtra("PID", 0);
            SqliteDB sdb = new SqliteDB(pid);

            string drname = "0";
            if (db.connect())
            {
                MySqlDataReader rd = db.executeQuery("select * from `users` where id = " + did + "");
                if (rd.Read())
                    drname = rd.GetString("Name");
                rd.Close();
            }

            notification(did, "Date Remainder", "Dr : "+drname+" at "+time+" In "+loc, context);

            db.exectueDML("delete from `dates` where `time` = '" + time + "'");
        }

        public void notification(int num, string title, string content, Context con)
        {
            Notification.Builder bd = new Notification.Builder(con);
            bd.SetContentTitle(title);
            bd.SetContentText(content);
            bd.SetSmallIcon(Resource.Drawable.icon1);
            bd.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));
            Notification not = bd.Build();

            NotificationManager man = (NotificationManager)con.GetSystemService(Context.NotificationService);

            man.Notify(num, not);
        }
        /*
        public void setAlarm(int num, string dname, int doses, int pill, int time, int pid, Context con)
        {
            AlarmManager man = (AlarmManager)con.GetSystemService(Context.AlarmService);
            Intent it = new Intent(con, typeof(AlarmReciver));
            it.PutExtra("name", dname);
            it.PutExtra("doses", "" + doses);
            it.PutExtra("pill", "" + pill);
            it.PutExtra("num", num);
            it.PutExtra("PID", pid);

            ISharedPreferences pre = PreferenceManager.GetDefaultSharedPreferences(con);

            int rc = pre.GetInt("remid", 0);

            ISharedPreferencesEditor edit = pre.Edit();
            edit.PutInt("remid", rc + 1);
            edit.Apply();

            PendingIntent pit = PendingIntent.GetBroadcast(con, rc, it, 0);
            man.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime() + time * 1000, pit);
        }*/
    }
}