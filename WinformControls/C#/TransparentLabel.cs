using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryBuilder.CustomControl
{
//A label that is transparent to mouse event. It will fall through.
    public class TransparentLabel : Label
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                m.Result = new IntPtr(-1);
                return;
            }
            base.WndProc(ref m);
        }
    }
}
