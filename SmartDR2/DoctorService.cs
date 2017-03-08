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
using System.Threading;
using System.Threading.Tasks;

namespace SmartDR2
{
    [Service]
    class DoctorService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {

        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            // countine
            new Task(() =>
            {
                while (true)
                {
                    Intent it = new Intent();
                    it.SetAction("com.blackburn95.doctor_service");
                    it.PutExtra("ID", intent.GetStringExtra("ID"));
                 //   SendBroadcast(it);

                    Thread.Sleep(10000);
                }
            }).Start();

            return StartCommandResult.Sticky;
        }
    }
}