using System.Windows.Forms;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    public class GroupButton : Button
    {
        internal Group m_Group;

        public Group Group
        {
            get
            {
                return m_Group;
            }

            set
            {
                m_Group = value;
            }
        }
    }
}
