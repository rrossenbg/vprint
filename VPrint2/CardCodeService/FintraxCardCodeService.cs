using System.ServiceProcess;

namespace CardCodeService
{
    public partial class FintraxCardCodeService : ServiceBase
    {
        public FintraxCardCodeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
