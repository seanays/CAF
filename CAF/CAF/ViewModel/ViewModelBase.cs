using System.ComponentModel;
using System.Runtime.CompilerServices;
using CAF.Annotations;
using CAF.CAD;

namespace CAF.ViewModel
{
    public class ViewModelBase: INotifyPropertyChanged
    {
        public RelayCommand CreateCubeCommand { get; set; }

        public ViewModelBase()
        {
            CreateCubeCommand = new RelayCommand(CreateCube);
        }

        private void CreateCube(object obj)
        {

            double dimX = 0, dimY = 0, dimZ = 0;
            //
            CADServices cadServices = new CADServices();
            CADServices.CreateCube(dimX, dimY, dimZ);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}