namespace ISoft.RichReports.UI
{
    public class RichReportProperties : IRichReportProperties
    {
        private string _fileName;
        private string _serverName;
        private string _userName;
        private string _databaseName;
        private string _appVersion;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                _serverName = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
            }
        }

        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
            set
            {
                _appVersion = value;
            }
        }
    }
}
