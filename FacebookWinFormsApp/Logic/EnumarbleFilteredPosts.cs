using System.Collections;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    internal class EnumarbleFilteredPosts : IEnumerable<Post>
    {
        internal List<Post> m_FilteredPosts = new List<Post>();

        public IEnumerator<Post> GetEnumerator()
        {
            foreach (Post post in m_FilteredPosts)
            {
                yield return post;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddPost(Post i_Post)
        {
            m_FilteredPosts.Add(i_Post);
        }
    }
}
