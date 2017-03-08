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

namespace SmartDR2
{
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent it)
        {
            string name = it.GetStringExtra("name");
            int doses = Convert.ToInt32(it.GetStringExtra("doses"));
            int pill = Convert.ToInt32(it.GetStringExtra("pill"));
            int time = Convert.ToInt32(it.GetStringExtra("time"));
            int num = it.GetIntExtra("num", 0);
            int rc = it.GetIntExtra("rc", 0);

            DBManage db = new DBManage();
            int pid = it.GetIntExtra("PID", 0);
            SqliteDB sdb = new SqliteDB(pid);
            bool f = db.connect();

            doses = sdb.getNumDoses(name, pid);

            notification(num, "Drug Remainder", "Take " + pill + " of Durg " + name, context);
            doses--;
                        
            if (f)
            {
                if (doses <= 0)
                {
                    db.exectueDML("update `Drugs` set `num_of_doses` = " + doses + " where `Drugs`.`drug_name` = '" + name + "' and patient_id = " + pid);
                   // db.exectueDML("delete from `Drugs` where drug_name = '" + name + "' and patient_id = "+pid);
                    sdb.deleteDrug(name,pid);
                    doses = 0;
                }
                else db.exectueDML("update `Drugs` set `num_of_doses` = " + doses + " where `Drugs`.`drug_name` = '" + name + "' and patient_id = "+pid);
            }
            if (doses > 0)
            {
                setAlarm(num, name, doses, pill, time, pid, context);
                sdb.updateDrug(name, doses, time, pill, pid);
            }
           // else sdb.deleteDrug(name);
        }

        public void notification(int num,string title,string content,Context con)
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

        public void setAlarm(int num, string dname, int doses, int pill, int time,int pid,Context con)
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
        }
    }
}