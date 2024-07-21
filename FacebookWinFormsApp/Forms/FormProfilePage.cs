using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Logic;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormProfilePage : Form
    {
        private readonly Facade r_Facade;
        public readonly User r_LoggedInUser;
        private readonly List<string> r_SelectedAlbumPhotos = new List<string>();
        private IEnumerator<Post> m_Iterator;
        private List<GroupButton> m_GuessGroupBtns = new List<GroupButton>(3);
        private GroupButton m_CorrectGroupButton = new GroupButton();
        private int m_AlbumPhotoIndex;

        public FormProfilePage(User i_LoggedInUser)
        {
            r_LoggedInUser = i_LoggedInUser;
            r_Facade = Facade.Instance;
            InitializeComponent();
            initProfile();
            r_Facade.InitLoggedInUser(i_LoggedInUser);
        }

        public class FilterList : List<FilterList.FilterItem>
        {            
            public class FilterItem
            {
                public Action CommandAction { get; set; }
                public string Text { get; set; }
            }
        }

        private void initProfile()
        {
            userNameText.Text = r_LoggedInUser.Name;
            profilePictureBox.Image = r_LoggedInUser.ImageNormal;
        }

        private void initFilteredListBox()
        {
            List<FilterList.FilterItem> filteredItemsList = new List<FilterList.FilterItem>();
            
            postsFilterListBox.DisplayMember = "Text";
            filteredItemsList.Add(new FilterList.FilterItem { Text = "Posts from your BDay", CommandAction = () => r_Facade.m_NostalgicFeature.m_Filter.FilterStrategyMethod = (post) => post.CreatedTime.ToString() == r_LoggedInUser.Birthday});
            filteredItemsList.Add(new FilterList.FilterItem { Text = "Posts from the last year", CommandAction = () => r_Facade.m_NostalgicFeature.m_Filter.FilterStrategyMethod = (post) => post.CreatedTime >= DateTime.Now.AddYears(-1) });
            filteredItemsList.Add(new FilterList.FilterItem { Text = "Posts where you felt excited", CommandAction = () => r_Facade.m_NostalgicFeature.m_Filter.FilterStrategyMethod = (post) => post.Message.IndexOf("!") > -1 });
            filteredItemsList.Add(new FilterList.FilterItem { Text = "All posts on my profile page", CommandAction = () => r_Facade.m_NostalgicFeature.m_Filter.FilterStrategyMethod = (post) => post.Message != null });
            foreach (FilterList.FilterItem item in filteredItemsList)
            {
                postsFilterListBox.Items.Add(item);
            }
        }

        private void newPostBtn_Click(object sender, EventArgs e)
        {
            try
            {
                r_LoggedInUser.PostStatus(postTextBox.Text);
            }
            catch
            {
                MessageBox.Show(string.Format("The Post couldn't be uploaded to your page,{0} it was uploaded successfully to recent posts in post tab :)", Environment.NewLine));
            }

            recentPostsListBox.Items.Add(postTextBox.Text);
            postTextBox.Text = string.Empty;
        }

        private void albumsBtn_Click(object sender, EventArgs e)
        {
            albumsTab.Text = "Albums";
            tabController.SelectTab("albumsTab");
            new Thread(fetchAlbums).Start();
        }

        private void fetchAlbums()
        {
            foreach (Album album in r_LoggedInUser.Albums)
            {
                albumListBox.Invoke(new Action(() =>
                {
                    albumListBox.DisplayMember = "Name";
                    albumListBox.Items.Add(album);
                }));
            }
        }

        private void albumListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Album selectedAlbum = albumListBox.SelectedItem as Album;

            m_AlbumPhotoIndex = 0;
            r_SelectedAlbumPhotos.Clear();
            foreach (Photo photo in selectedAlbum.Photos)
            {
                r_SelectedAlbumPhotos.Add(photo.PictureNormalURL);
            }

            try
            {
                displayPictureFromAlbum(r_SelectedAlbumPhotos[m_AlbumPhotoIndex]);
            }
            catch
            {
                MessageBox.Show("Album can't be displayed");
            }
        }

        private void displayPictureFromAlbum(string i_PictureAlbumURL)
        {
            if (i_PictureAlbumURL != null)
            {
                imageNormalPictureBox.LoadAsync(i_PictureAlbumURL);
            }
            else
            {
                imageNormalPictureBox.Image = imageNormalPictureBox.ErrorImage;
            }
        }

        private void fetchGroups()
        {
            groupsListBox.Invoke(new Action(() => groupBindingSource.DataSource = r_LoggedInUser.Groups));
        }

        private void groupsBtn_Click(object sender, EventArgs e)
        {
            groupsTab.Text = "Groups";
            tabController.SelectedTab = groupsTab;
            new Thread(fetchGroups).Start();
        }

        private void friendsBtn_Click(object sender, EventArgs e)
        {
            friendsTab.Text = "Friends";
            tabController.SelectedTab = friendsTab;
            new Thread(fetchFriends).Start();
        }

        private void fetchFriends()
        {
            try
            {
                friendsListBox.Invoke(new Action(() => membersBindingSource.DataSource = r_LoggedInUser.Friends));
            }
            catch
            {
                 friendBirthdayTextBox.Invoke(new Action(() => friendBirthdayTextBox.Text = "You don't have permission to do that :)"));
            }
        }

        private void backToLoginBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void eventsBtn_Click(object sender, EventArgs e)
        {
            eventsTab.Text = "Events";
            tabController.SelectedTab = eventsTab;
            new Thread(fetchEvents).Start();
        }

        private void fetchEvents()
        {
            foreach (Event e in r_LoggedInUser.Events)
            {
                eventsListBox.Invoke(new Action(() =>
                {
                    eventsListBox.DisplayMember = "Name";
                    eventsListBox.Items.Add(e);
                }
                ));
            }
        }

        private void eventsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Event selectedEvent = eventsListBox.SelectedItem as Event;

            try
            {
                eventsPostTextBox.Text = selectedEvent.WallPosts[0].Message;
            }
            catch
            {
                eventsPostTextBox.Text = "You don't have permission to do that :)";
            }
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            if (m_AlbumPhotoIndex < r_SelectedAlbumPhotos.Count - 1)
            {
                m_AlbumPhotoIndex++;
                displayPictureFromAlbum(r_SelectedAlbumPhotos[m_AlbumPhotoIndex]);
            }
        }

        private void postsBtn_Click(object sender, EventArgs e)
        {
            postsTab.Text = "Posts";
            tabController.SelectedTab = postsTab;
            if (postsListBox.Items.Count == 0)
            {
                new Thread(fetchPosts).Start();
            }
        }

        private void fetchPosts()
        {            
            try
            {
                foreach (Post post in r_LoggedInUser.Posts)
                {
                    if (post.Message != null)
                    {
                        postsListBox.Invoke(new Action(() =>
                        {
                            postsListBox.DisplayMember = "Message";
                            postsListBox.Items.Add(post);
                        }
                        ));
                    }
                }
            }
            catch
            {
                recentPostsListBox.Invoke(new Action(() => recentPostsListBox.Text = "You don't have permission to do that :)"));
            }
        }

        private void nostalgicFeatureBtn_Click(object sender, EventArgs e)
        {
            nostalgiaTab.Text = "Nostalgia";
            tabController.SelectedTab = nostalgiaTab;
            Update();
            initFilteredListBox();
        }

        private void shareNostalgiaBtn_Click(object sender, EventArgs e)
        {
            string newPost = string.Format("{0}I found an old memory from {1}:{0}{2}", Environment.NewLine, nostalgicPostDateBox.Text, nostalgicPostBox.Text);
            
            MessageBox.Show(string.Format("Your nostalgic post was uploaded to your profile:{0}", newPost));
            recentPostsListBox.Items.Add(newPost);
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            if (m_AlbumPhotoIndex >= 1)
            {
                m_AlbumPhotoIndex--;
                displayPictureFromAlbum(r_SelectedAlbumPhotos[m_AlbumPhotoIndex]);
            }
        }

        private void guessGroupFeatureBtn_Click(object sender, EventArgs e)
        {
            guessTheGroupTab.Text = "Guess The Group";
            tabController.SelectedTab = guessTheGroupTab;
            generateButtonsAndPicture();
        }

        private void generateButtonsAndPicture()
        {
            int buttonHeight = 80;
            int buttonWidth = 160;
            int buttonTop = 450;
            int buttonLeft = 80;
            int betweenButtons = 100;
            
            for (int i = 0; i < 3; i++)
            { 
                GroupButton guessGroupBtn = new GroupButton();
                guessGroupBtn.Width = buttonWidth;
                guessGroupBtn.Height = buttonHeight;
                guessGroupBtn.Top = buttonTop;
                guessGroupBtn.Left = buttonLeft;
                guessGroupBtn.Group = r_Facade.m_GuessGroupFeature.m_ChosenGroups[i];
                guessGroupBtn.BackColor = Color.White;
                guessGroupBtn.Text = r_Facade.m_GuessGroupFeature.m_ChosenGroups[i].Name;
                guessGroupBtn.Font = new Font("Calibri", 12F);
                guessGroupBtn.Click += groupGuessButton_Click;
                m_GuessGroupBtns.Add(guessGroupBtn);
                buttonLeft += betweenButtons + buttonWidth;
                if (guessGroupBtn.Group == r_Facade.m_GuessGroupFeature.m_CorrectGroup)
                {
                    m_CorrectGroupButton = guessGroupBtn;
                    guessTheGroupPictureBox.Image = m_CorrectGroupButton.Group.ImageLarge;
                    guessTheGroupPictureBox.Update();
                }
            }

            foreach (GroupButton button in m_GuessGroupBtns)
            {
                guessTheGroupTab.Controls.Add(button);
            }
        }

        private void updateGroupsGameResult(GroupButton i_Button)
        { 
            bool isCorrect = r_Facade.CheckGroupGuess(i_Button.Group);

            if (isCorrect)
            {
                i_Button.BackColor = Color.Green;
            }
            else
            {
                i_Button.BackColor = Color.Red;
                m_CorrectGroupButton.BackColor = Color.Green;
                m_CorrectGroupButton.Update();
            }

            i_Button.Update();
            Update();
        }

        private void groupGuessButton_Click(object sender, EventArgs e)
        {
            GroupButton guessGroupBtn = sender as GroupButton;
           
            if (guessGroupBtn != null)
            {
                updateGroupsGameResult(guessGroupBtn);
                Thread.Sleep(1000);
                initGameTurn();
            }
        }

        private void initGameTurn()
        {
            clearAfterTurn();
            r_Facade.SetRandomGroupsToGuess();
            generateButtonsAndPicture();
        }

        private void clearAfterTurn()
        {
            foreach (GroupButton button in m_GuessGroupBtns)
            {
                guessTheGroupTab.Controls.Remove(button);
            }

            guessTheGroupPictureBox.Image = null;
            m_GuessGroupBtns.Clear();
            Update();
        }

        private void endGuessGroupGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("Game Over! {0} Your score is: * * * {1} * * *{2} Out of {3} games :)", 
                Environment.NewLine, r_Facade.m_GuessGroupFeature.m_GroupsGameScore,Environment.NewLine, r_Facade.m_GuessGroupFeature.m_GroupGameNumOfGames));
            r_Facade.InitNumOfGamesAndScore();
            clearAfterTurn();
            tabController.SelectedTab = profileTab;
        }

        private void postsFilterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterList.FilterItem selectedFilter = postsFilterListBox.SelectedItem as FilterList.FilterItem;
            
            clearNostalgicPageByFilter();
            selectedFilter.CommandAction.Invoke();
            r_Facade.m_NostalgicFeature.GenerateFilteredPostsList();
            m_Iterator = r_Facade.m_NostalgicFeature.m_EnumarbleFilteredPosts.GetEnumerator();
            if (r_Facade.m_NostalgicFeature.m_EnumarbleFilteredPosts.m_FilteredPosts.Count > 0)
            {
                nostalgicPostBtnNext.Visible = true;
                shareNostalgiaBtn.Visible = true;
            }
            else
            {
                noPostsFoundTitle.Visible = true;
            }
        }

        private void nostalgicPostBtnNext_Click(object sender, EventArgs e)
        {            
            if (m_Iterator.MoveNext())
            {
                nostalgicPostDateBox.Text = string.Format("On {0} you posted:", m_Iterator.Current.UpdateTime.ToString());
                nostalgicPostBox.Text = m_Iterator.Current.Message;
            }
        }

        private void clearNostalgicPageByFilter()
        {
            noPostsFoundTitle.Visible = false;
            nostalgicPostBtnNext.Visible = false;
            shareNostalgiaBtn.Visible = false;
            nostalgicPostDateBox.Text = string.Empty;
            nostalgicPostBox.Text = string.Empty;
        }
    }
}
