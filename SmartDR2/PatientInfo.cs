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
    class PatientInfo
    {
        public int id;
        public string name, email, mobile;

        public PatientInfo(int i, string n, string e, string p)
        {
            id = i;
            name = n;
            email = e;
            mobile = p;
        }
    }
}