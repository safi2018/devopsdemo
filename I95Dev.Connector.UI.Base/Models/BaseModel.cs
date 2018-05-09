using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace I95Dev.Connector.UI.Base.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val)) return false;

            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}