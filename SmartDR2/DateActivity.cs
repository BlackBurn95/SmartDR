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
    [Activity(Label = "DateActivity",ParentActivity=typeof(PatientActivity))]
    public class DateActivity : Activity
    {
        private int pid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dates_layout);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            ListView list = FindViewById<ListView>(Resource.Id.dates_listview);

            pid = Intent.GetIntExtra("PID", 0);
            SqliteDB db = new SqliteDB(pid);
            List<DateInfo> data = db.selectDatesByPId(pid);

            List<Item> l = new List<Item>();

            for (int i = 0; i < data.Count(); i++)
                l.Add(new Item("Doctor Name: "+data[i].drname, "Location : "+data[i].location, "Time and Date : "+data[i].time));

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

            public Item(string n, string s1,string s2)
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
                    v = context.LayoutInflater.Inflate(Resource.Layout.dates_ticket, null);

                v.FindViewById<TextView>(Resource.Id.tick_dates_name).Text = item.name;
                v.FindViewById<TextView>(Resource.Id.tick_dates_location).Text = item.sub1;
                v.FindViewById<TextView>(Resource.Id.tick_dates_time).Text = item.sub2;

                return v;
            }
        }
    }
}