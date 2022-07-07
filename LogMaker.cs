using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSubscriberApp
{
    public class LogMaker
    {
        public static string WriteLog(string log)
        {
            string strCurDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string retLog = string.Format("[{0}] {1}", strCurDateTime, log);

            Console.WriteLine(retLog);
            return retLog;
        }
    }
}
