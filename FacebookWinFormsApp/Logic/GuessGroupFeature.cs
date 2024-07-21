using System;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace Logic
{
    public class GuessGroupFeature
    {
        private readonly User r_LoggedInUser;
        private readonly List<Group> r_AllGroupsToGuess = new List<Group>();
        private readonly Random r_Random = new Random();
        public int m_GroupsGameScore;
        public int m_GroupGameNumOfGames;
        public List<Group> m_ChosenGroups = new List<Group>(3);
        public Group m_CorrectGroup;

        internal GuessGroupFeature(User i_LoggedInUser)
        {
            r_LoggedInUser = i_LoggedInUser;
            GenerateAllGroupsList();
            SetRandomGroupsToGuess();
        }

        internal void GenerateAllGroupsList()
        {
            m_GroupsGameScore = 0;
            m_GroupGameNumOfGames = 0;

            foreach (Group group in r_LoggedInUser.Groups)
            {
                r_AllGroupsToGuess.Add(group);
            }
        }

        internal void SetRandomGroupsToGuess()
        {
            int randomIndex = r_Random.Next(0, r_AllGroupsToGuess.Count);

            m_ChosenGroups.Clear();
            for (int i = 0; i < 3; i++)
            {
                while (m_ChosenGroups.Contains(r_AllGroupsToGuess[randomIndex]))
                {
                    randomIndex = r_Random.Next(0, r_AllGroupsToGuess.Count);
                }

                m_ChosenGroups.Add(r_AllGroupsToGuess[randomIndex]);
            }

            ChooseCorrctGroup();
        }

        internal void ChooseCorrctGroup()
        {
            m_CorrectGroup = m_ChosenGroups[r_Random.Next(0, m_ChosenGroups.Count)];
        }

        internal bool UpdateGroupsGameResult(Group i_GuessedGroup)
        {
            bool isCorrect = true;

            if (i_GuessedGroup == m_CorrectGroup)
            {
                m_GroupsGameScore++;
            }
            else
            {
                isCorrect = false;
            }

            m_GroupGameNumOfGames++;

            return isCorrect;
        }

        internal void InitNumOfGamesAndScore()
        {
            m_GroupsGameScore = 0;
            m_GroupGameNumOfGames = 0;
        }
    }
}
