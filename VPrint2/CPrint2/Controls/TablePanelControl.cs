using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CPrint2.Controls
{
    public class TablePanelControl : SplitContainer
    {
        private readonly List<Control> m_List = new List<Control>();

        private TablePanelControl m_Active;

        public IEnumerable<Control> Controls2
        {
            get
            {
                return m_List;
            }
        }

        public TablePanelControl()
        {
            m_Active = this;
            Dock = DockStyle.Fill;
            Orientation = Orientation.Horizontal;
        }

        public void AddRows(IList<Control> cnts)
        {
            m_List.AddRange(cnts);

            for (int i = 0; i < cnts.Count - 1; i++)
            {
                var cnt = cnts[i];

                m_Active.Panel1.Controls.Add(cnt);
                m_Active.Panel2.Controls.Add((i + 1 < cnts.Count - 1) ? m_Active = new TablePanelControl() : cnts[i + 1]);
            }
        }
    }
}
