using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace btwm
{
    class Display
    {
        public Screen Screen;
        public string DisplayedWorkspace;

        public Display(Screen Screen, string Workspace)
        {
            this.Screen = Screen;
            DisplayedWorkspace = Workspace;
        }

		public override string ToString ()
		{
			return "{Screen=" + Screen.ToString() + ",DisplayedWorkspace=" + DisplayedWorkspace + "}";
		}
    }
}
