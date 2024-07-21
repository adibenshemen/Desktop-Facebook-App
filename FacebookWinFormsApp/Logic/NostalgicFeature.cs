using System;
using System.Threading;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    public class NostalgicFeature
    {
        private readonly User r_LoggedInUser;
        private readonly List<Post> r_AllPosts = new List<Post>();
        internal EnumarbleFilteredPosts m_EnumarbleFilteredPosts = new EnumarbleFilteredPosts();
        public PostsFilter m_Filter = new PostsFilter();

        internal NostalgicFeature(User i_LoggedInUser)
        {
            r_LoggedInUser = i_LoggedInUser;
            GenerateAllPostsList();
        }

        internal void GenerateAllPostsList()
        {
            foreach (Post post in r_LoggedInUser.Posts)
            {
                if (post.Message != null)
                {
                    r_AllPosts.Add(post);
                }
            }
        }

        internal void GenerateFilteredPostsList()
        {
            m_EnumarbleFilteredPosts.m_FilteredPosts.Clear();
            foreach (Post post in r_AllPosts)
            {
                if (m_Filter.FilterStrategyMethod(post))
                {
                    m_EnumarbleFilteredPosts.AddPost(post);
                }
            }
        }
    }
}
