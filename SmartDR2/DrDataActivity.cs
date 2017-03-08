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

namespace SmartDR2
{
    [Activity(Label = "DrDataActivity", ParentActivity = typeof(Doctor))]
    public class DrDataActivity : Activity
    {
        private int pid,drid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dr_date_layout);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            ListView list = FindViewById<ListView>(Resource.Id.dates_listview_dr);

            drid = Intent.GetIntExtra("DRID", 0);
            SqliteDB db = new SqliteDB(drid);
            List<DateInfo> data = db.selectDates(drid);

            List<Item> l = new List<Item>();

            for (int i = 0; i < data.Count(); i++)
                l.Add(new Item("Patient ID: " + data[i].id, "Location : " + data[i].location, "Time and Date : " + data[i].time));

            list.Adapter = new MyAdapter(this, l);
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
        private class Item
        {
            public string name, sub1, sub2;

            public Item(string n, string s1, string s2)
            {
                name = n;
                sub1 = s1;
                sub2 = s2;
            }
        }

        private class MyAdapter : BaseAdapter<Item>
        {
            List<Item> l;
            Activity context;

            public MyAdapter(Activity con, List<Item> items)
                : base()
            {
                context = con;
                l = items;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override Item this[int position]
            {
                get { return l[position]; }
            }

            public override int Count
            {
                get { return l.Count; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var item = l[position];
                View v = convertView;
                if (v == null)
                    v = context.LayoutInflater.Inflate(Resource.Layout.dr_date_ticket, null);

                v.FindViewById<TextView>(Resource.Id.tick_dates_name_dr).Text = item.name;
                v.FindViewById<TextView>(Resource.Id.tick_dates_location_dr).Text = item.sub1;
                v.FindViewById<TextView>(Resource.Id.tick_dates_time_dr).Text = item.sub2;

                return v;
            }
        }
    }

}