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
    [Activity(Label = "PatientData", ParentActivity = typeof(PatientView))]
    public class PatientData : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.patient_view_layout);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            ListView list = FindViewById<ListView>(Resource.Id.listView2);///

            SqliteDB db = new SqliteDB(Intent.GetIntExtra("DRID",0));
            List<DrugInfo> data = db.selectDrugByID(Intent.GetIntExtra("ID",0));

            List<Item> l = new List<Item>();

          //  Toast.MakeText(this, "Num of Pat : " + l.Count(), ToastLength.Short).Show();
            for (int i = 0; i < data.Count(); i++)
                l.Add(new Item(data[i].numDoses,data[i].time,data[i].numPill,data[i].name));

            list.Adapter = new MyAdapter(this, l);
            //list.ItemClick += OnListItemClick;
        }
        /*
        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lv = sender as ListView;
            var t = l[e.Position];

            Intent it = new Intent(this, typeof(PatientView));
            it.PutExtra("ID", t.id);

            StartActivity(it);
        }
        */
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
            public int doses,time,pill;
            public string name;

            public Item(int d,int t,int p,string n)
            {
                doses = d;
                time = t;
                pill = p;
                name = n;
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
                    v = context.LayoutInflater.Inflate(Resource.Layout.ticket3, null);

                v.FindViewById<TextView>(Resource.Id.tick3_drug_name).Text = "Drug Name : "+item.name;
                v.FindViewById<TextView>(Resource.Id.tick3_doses).Text = "Doses Remaining : " + item.doses;
                v.FindViewById<TextView>(Resource.Id.tick3_time).Text = "Time each "+item.time+" hour";
                v.FindViewById<TextView>(Resource.Id.tick3_pill).Text = "Pills " + item.pill;
                
                return v;
            }
        }
    }
}