using UnityEngine;

namespace Saver
{
    /// <summary>
    ///     XML数据
    /// </summary>
    public static class XMLDataBase
    {
        /// <summary>
        ///     存档名
        /// </summary>
        public static string DataName = "UserData";

        /// <summary>
        ///     存档路径
        /// </summary>
        public static string XMLPath => Application.dataPath + "/Data/";
    }
}