using System;

namespace Common.Entity
{
    public class userEntity
    {
        public bool rFolderNeedFlg;

        public string userID { get; set; }
        public string password { get; set; }

        public string transType { get; set; }
        public string dealType { get; set; }
    }
}
