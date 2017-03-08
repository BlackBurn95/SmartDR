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
    class DrugInfo
    {
        public string name;
        public int numDoses, time, numPill;

        public DrugInfo(string n, int num, int t,int num2)
        {
            name = n;
            numDoses = num;
            time = t;
            numPill = num2;
        }
    }
}