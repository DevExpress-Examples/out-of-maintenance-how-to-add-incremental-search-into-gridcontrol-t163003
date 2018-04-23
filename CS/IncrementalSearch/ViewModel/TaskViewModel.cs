using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncrementalSearch
{
    public class TaskViewModel
    {

        private ObservableCollection<Task> _TaskData;

        public ObservableCollection<Task> TaskData
        {
            get
            {
                if (_TaskData == null)
                {
                    _TaskData = new ObservableCollection<Task>();

                    for (int i = 0; i < 30; i++)
                    {
                        _TaskData.Add(new Task() { Name = "Task " + i, Number = i, Date = new DateTime(2014, 10, new Random().Next(1, 31)), IsCompleted = i % 2 != 0 });
                    }
                }
                return _TaskData;
            }
        }
    }

    public class Task : INotifyPropertyChanged
    {
        private string _Name;
        private int _Number;
        private DateTime _Date;
        private bool _IsCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                OnPropertyChanged("Number");
            }
        }

        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
                OnPropertyChanged("Date");
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _IsCompleted;
            }
            set
            {
                _IsCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }
    }
}
