
using System;
using System.Web;
using System.Web.UI;

namespace testPrj
{
	public partial class Default : System.Web.UI.Page
	{
		
		public virtual void button1Clicked (object sender, EventArgs args)
		{
			button1.Text = System.IO.Path.GetFileName(Request.Path);

		}

	}
}

