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
    class DateInfo
    {
        public string drname, location, time;
        public int id;

        public DateInfo(string d, string l, string t,int id)
        {
            drname = d;
            location = l;
            time = t;
            this.id = id;
        }
    }
}