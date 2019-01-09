//---------------------------------------------------------------------------------------------------
// Players Data Access
//---------------------------------------------------------------------------------------------------
// Created: 08 Jan 2018, Alan G. Stewart
// Changed: 
//---------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using PingPong.DBLib;

namespace PingPong.DALib
{
    public class Players
    {
        private ConstructorInfo oConstructor = Player.MyConstructor;

        public List<Player> List()
        {
            DbCommand oCmd = null;
            List<Player> oList = null;

            oCmd = Utility.CreateSPCommand(Globals.ConnectionName, "Players_List");
            oList = Utility.ReadList<Player>(ref oCmd, ref oConstructor);

            return oList;
        }

        public Player Read(int PlayerID)
        {
            DbCommand oCmd = null;
            Player oItem = null;

            oCmd = Utility.CreateSPCommand(Globals.ConnectionName, "Players_Read");
            Utility.AddParameter(ref oCmd, "@PlayerID", DbType.Int32, PlayerID);
            oItem = (Player)Utility.ReadSingle(ref oCmd, ref oConstructor);

            return oItem;
        }

        public void Save(Player player)
        {
            DbCommand oCmd = null;

            oCmd = Utility.CreateSPCommand(Globals.ConnectionName, "Players_Save");
            Utility.AddParameter(ref oCmd, "@PlayerID", DbType.Int32, player.PlayerID);
            Utility.AddParameter(ref oCmd, "@FirstName", DbType.String, player.FirstName, 50);
            Utility.AddParameter(ref oCmd, "@LastName", DbType.String, player.LastName, 50);
            Utility.AddParameter(ref oCmd, "@Age", DbType.Int32, player.Age);
            Utility.AddParameter(ref oCmd, "@SkillLevel", DbType.String, player.SkillLevel, 20);
            Utility.AddParameter(ref oCmd, "@Email", DbType.String, player.Email, 128);
            Utility.ExecuteNonQuery(oCmd);

            return;
        }

        public void Remove(int PlayerID)
        {
            DbCommand oCmd = null;

            oCmd = Utility.CreateSPCommand(Globals.ConnectionName, "Players_Remove");
            Utility.AddParameter(ref oCmd, "@PlayerID", DbType.Int32, PlayerID);
            Utility.ExecuteNonQuery(oCmd);

            return;
        }
    }
}
