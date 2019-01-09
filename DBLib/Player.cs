//---------------------------------------------------------------------------------------------------
// Players Data Class
//---------------------------------------------------------------------------------------------------
// Created: 08 Jan 2018, Alan G. Stewart
// Changed: 
//---------------------------------------------------------------------------------------------------
using System;
using System.Data.Common;
using System.Reflection;

namespace PingPong.DBLib
{
    public class Player
    {
        // Table Fields
        public int PlayerID { get; set; }              // INTEGER
        public string FirstName { get; set; }         // VARCHAR(50)
        public string LastName { get; set; }          // VARCHAR(50)
        public int Age { get; set; }                  // INTEGER
        public string SkillLevel { get; set; }        // VARCHAR(20)
        public string Email { get; set; }             // VARCHAR(128);

        #region "Shared Properties"
        //** Reflection is expensive so these properties cache useful data
        //** Set up a shared FieldInfo array for our data fields
        private static Array static_Fields;
        public static Array MyFields
        {
            get
            {
                if (static_Fields == null)
                {
                    static_Fields = typeof(Player).GetProperties();
                }
                return static_Fields;
            }
        }
        //** The Constructor to be used with a DataReader
        private static ConstructorInfo static_Constructor;
        public static ConstructorInfo MyConstructor
        {
            get
            {

                if (static_Constructor == null)
                {
                    Type[] oSignature = { typeof(DbDataReader) };
                    static_Constructor = typeof(Player).GetConstructor(oSignature);
                }
                return static_Constructor;
            }
        }
        #endregion

        #region "Public Methods"

        /// <summary>
        /// Create a new instance initialized with default values.
        /// </summary>
        public Player()
        {
            object me = this;
            Array aFields = MyFields;
            Utility.InitFields(ref me, ref aFields);
        }
        /// <summary>
        /// Create a new instance initialized from the current row in a datareader.
        /// </summary>
        public Player(DbDataReader Reader)
        {
            object me = this;
            Array aFields = MyFields;
            Utility.ReadFields(ref me, ref aFields, ref Reader);
        }
        #endregion
    }
}