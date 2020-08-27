using System.Collections.Generic;

namespace PertEstimationTool.Enums
{
    public class EstimationTimeFormats
    {
        private string _hour = Properties.Resources.hour;
        public string Hour { get => _hour; }

        private string _day = Properties.Resources.day;
        public string Day { get => _day; }

        private string _week = Properties.Resources.week;
        public string Week { get => _week; }

        private string _month = Properties.Resources.month;
        public string Month { get => _month; }

        private string _year = Properties.Resources.year;
        public string Year { get => _year; }

        private List<string> _supportedTimeFormats;
        public List<string> SupportedTimeFormats { get => _supportedTimeFormats; }

        public EstimationTimeFormats()
        {
            if (_supportedTimeFormats == null)
            {
                _supportedTimeFormats = new List<string>();

                _supportedTimeFormats.Add(_hour);
                _supportedTimeFormats.Add(_day);
                _supportedTimeFormats.Add(_week);
                _supportedTimeFormats.Add(_month);
                _supportedTimeFormats.Add(_year);
            }
        }
    }
}
