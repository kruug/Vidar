using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidar
{
    internal class User
    {
        public string name { get; set; }
        public string status { get; set; }
        public int userID { get; set; }
        public int level { get; set; }
        public int age { get; set; }
        public float reputation { get; set; }
        public int? cartelId { get; set; } = 0;
        public int jailRelease { get; set; }
        public int hospitalRelease { get; set; }
        public int currentLife { get; set; }
        public int maxLife { get; set; }
        public int attacksWon { get; set; }
        public int defendsLost { get; set; }
        public int lastActive { get; set; }
        public string userType { get; set; }
        public int friendCount { get; set; }
        public int enemyCount { get; set; }
        public string currentBountyReward { get; set; }
    }
}
