using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace WpfApplication1
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        public PersonViewModel()
        {         
            People = new ObservableCollection<Person>();
            DodajCommand = new DelegateCommand(Dodaj, CzyDodaj);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand DodajCommand { get; private set; }

        private void Dodaj()
        {
            var person = new Person
            {
                Name = Name,
                Surname = Surname
            };
            People.Add(person);
        }

        private bool CzyDodaj()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname);
        }

        public ObservableCollection<Person> People { get; set; } 

        private string _name;
        public string Name
        {
            get { return _name;}
            set
            {
                _name = value;
                OnPropertyChanged();
                DodajCommand.RaiseCanExecuteChanged();
            }
        }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                OnPropertyChanged();
                DodajCommand.RaiseCanExecuteChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}