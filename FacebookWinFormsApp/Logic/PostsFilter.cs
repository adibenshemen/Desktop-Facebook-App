using System;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    public class PostsFilter
    {
        private Func<Post, bool> m_FilterStrategyMethod;

        public Func<Post, bool> FilterStrategyMethod
        {
            get
            {
                return m_FilterStrategyMethod;
            }
            set
            {
                m_FilterStrategyMethod = value;
            }
        }
    }
}
