using System;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public partial class FormLogin : Form
    {
        private User m_LoggedUser;
        private LoginResult m_LoginResult;
        private FormProfilePage m_FormProfilePage;

        public FormLogin()
        {
            InitializeComponent();
            FacebookService.s_CollectionLimit = 100;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("adibenshemen@gmail.com");
            m_LoginResult = FacebookService.Login(
                // "1990619577793916",
                "1450160541956417",
                "email",
                "public_profile",
                "user_birthday",
                "user_events",
                "user_likes",
                "user_location",
                "user_posts",
                "user_link",
                "user_photos",
                "user_friends",
                "groups_access_member_info"
                );

            if (!string.IsNullOrEmpty(m_LoginResult.AccessToken))
            {
                m_LoggedUser = m_LoginResult.LoggedInUser;
                buttonLogin.Text = string.Format("Logged in as {0}", m_LoginResult.LoggedInUser.Name);
                m_FormProfilePage = new FormProfilePage(m_LoggedUser);
                m_FormProfilePage.Show();
            }
            else
            {
                MessageBox.Show("Can't connect to facebook. Login failed");
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            if (m_LoggedUser != null)
            {
                FacebookService.LogoutWithUI();
                buttonLogin.Text = "Login";
            }
            else
            {
                MessageBox.Show("You need to login first");
            }
        }
    }
}
 