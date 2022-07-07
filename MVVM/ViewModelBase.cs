using System;
using System.ComponentModel;
using System.Diagnostics;

namespace MqttSubscriberApp
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 구현부
        public event PropertyChangedEventHandler PropertyChanged;

        // 각 Property 이름으로 지정해 놓고 UI 쪽으로 변경에 대한 이벤트 구현
        protected void NotifyPropertyChanged(string propertyName = "")
        {
            this.VerifyPropertyName(propertyName);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                throw new Exception(msg);
            }
        }
        #endregion
    }
}
