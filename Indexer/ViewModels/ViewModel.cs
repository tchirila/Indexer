using System.ComponentModel;
using System.Windows.Input;

namespace Indexer.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        Control control = new Control();

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModel()
        {
            ButtonExecuteLabel = "Execute";
            ContentExecute = "";
        }

        private string _ButtonExecuteLabel;
        public string ButtonExecuteLabel
        {
            get { return _ButtonExecuteLabel; }
            set
            {
                _ButtonExecuteLabel = value;
                onPropertyChanged("ButtonExecuteLabel");
            }
        }

        private string _ContentExecute;
        public string ContentExecute
        {
            get { return _ContentExecute; }
            set
            {
                _ContentExecute = value;
                onPropertyChanged("ContentExecute");
            }
        }

        //private ICommand _Btn_Execute_ClickCommand;
        //public ICommand Btn_Execute_ClickCommand
        //{
        //    get
        //    {
        //        if (_Btn_Execute_ClickCommand == null)
        //        {
        //            _Btn_Execute_ClickCommand = new Command(Btn_Execute_Click, Can_Btn_Execute_Click);
        //        }
        //        return _Btn_Execute_ClickCommand;
        //    }
        //    set { _Btn_Execute_ClickCommand = value; }
        //}

        private void Btn_Execute_ClickCommand()
        {
            ContentExecute = "Executing...";
            control.Run();
            ContentExecute = "Done";
        }

        private bool Can_Btn_Execute_Click() { return true; }
    }
}
