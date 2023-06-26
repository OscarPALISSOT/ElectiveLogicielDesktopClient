using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UserAdministration
{
    /// <summary>
    /// You opened it... I came.
    /// </summary>
    public class Box
    {
        private int _ID;
        public int ID
        {
            get { return _ID;}
        }
        private string text;

        public Box(int a, string b) {
            _ID = a;
            text = b;
        }

        public override string ToString()
        {
            return text;
        }
    }
}
