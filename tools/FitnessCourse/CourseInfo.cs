namespace Htggbb.FitnessCourse
{
    internal class ActionInfo
    {
        public string Name;
        public uint TimeSpan;
        public bool AutoChangeSides;
        public uint Countdown;
    }

    internal class CourseInfo
    {
        public string Name;
        public string SeriesName;
        public ActionInfo[] Actions;
    }
}