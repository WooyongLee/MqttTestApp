using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSubscriberApp
{
    public class ListViewModel
    {
        public string Time { get; set; }
        public string Information { get; set; }

        public ListViewModel(string strInfo)
        {
            this.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.Information = strInfo;
        }
    }
}
