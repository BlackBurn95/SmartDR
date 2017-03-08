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
    [Activity(Label = "PatientView", ParentActivity = typeof(Doctor))]
    public class PatientView : Activity
    {
        private List<Item> l;
        private int drid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.patient_view_layout2);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            ListView list = FindViewById<ListView>(Resource.Id.listView3);

            drid = Intent.GetIntExtra("DRID", 0);
            SqliteDB db = new SqliteDB(drid);

            List<PatientInfo> data = db.selectAllPatient();

            l = new List<Item>();

            for (int i = 0; i < data.Count(); i++)
                l.Add(new Item(data[i].id, data[i].name, data[i].email, data[i].mobile));

            list.Adapter = new MyAdapter(this, l);
            list.ItemClick += OnListItemClick;
        }
        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lv = sender as ListView;
            var t = l[e.Position];

            Intent it = new Intent(this, typeof(PatientData));
            it.PutExtra("ID", t.id);
            it.PutExtra("DRID", drid);
            StartActivity(it);
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
            public int id;
            public string name, email, mob;

            public Item(int i, string n, string s, string s2)
            {
                id = i;
                name = n;
                email = s;
                mob = s2;
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
                    v = context.LayoutInflater.Inflate(Resource.Layout.doc_ticket, null);

                v.FindViewById<TextView>(Resource.Id.tick2_name).Text = "ID : " + item.id + " , Name : " + item.name;
                v.FindViewById<TextView>(Resource.Id.tick2_email).Text = "Email : " + item.email;
                v.FindViewById<TextView>(Resource.Id.tick2_mob).Text = "Mobile : " + item.mob;

                return v;
            }
        }
    }
}