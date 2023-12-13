using Prism.Mvvm;

namespace DistGroup.Models
{
    public class Course : BindableBase
    {
        private int _nuCourseSeq;
        public int NuCourseSeq
        {
            get => _nuCourseSeq;
            set => SetProperty(ref _nuCourseSeq, value);
        }

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        public string PadCourse => CdCourse.PadLeft(3, '0');
    }
}
