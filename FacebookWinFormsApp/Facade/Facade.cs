using FacebookWrapper.ObjectModel;

namespace Logic
{
    public sealed class Facade
    {
        private Facade() { }

        private static readonly object sr_CreationLockContext = new object();
        private static Facade s_Instance = null;
        private User m_LoggedInUser;
        public NostalgicFeature m_NostalgicFeature;
        public GuessGroupFeature m_GuessGroupFeature;

        public static Facade Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (sr_CreationLockContext)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new Facade();
                        }
                    }
                }

                return s_Instance;
            }
        }
        
        public void InitLoggedInUser(User i_LoggedInUser)
        {
            m_LoggedInUser = i_LoggedInUser;
            m_GuessGroupFeature = new GuessGroupFeature(i_LoggedInUser);
            m_NostalgicFeature = new NostalgicFeature(i_LoggedInUser);
        }

        /*public Post FetchNostalgicPost()
        {
            m_NostalgicFeature.GenerateFilteredPostsList();
            return m_NostalgicFeature.PickRandomPost();
        }*/

        public bool CheckGroupGuess(Group i_GroupGuess)
        {
            return m_GuessGroupFeature.UpdateGroupsGameResult(i_GroupGuess);
        }

        public void InitNumOfGamesAndScore()
        {
            m_GuessGroupFeature.InitNumOfGamesAndScore();
        }

        public void SetRandomGroupsToGuess()
        {
            m_GuessGroupFeature.SetRandomGroupsToGuess();
        }
    }
}
