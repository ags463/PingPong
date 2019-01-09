//---------------------------------------------------------------------------------------------------
// Players Api Controller
//---------------------------------------------------------------------------------------------------
// Created: 08 Jan 2019, Alan G. Stewart
// Changed: 
//---------------------------------------------------------------------------------------------------
using System.Web.Http;
using System.Collections.Generic;
using PingPong.DALib;
using PingPong.DBLib;

namespace PingPong.Controllers
{
    public class PlayersController : ApiController
    {
        private Players oDA = new Players();

        [HttpGet]
        [Route("api/players/List")]
        public IHttpActionResult List()
        {
            List<Player> oList = null;

            oList = oDA.List();
            return Ok(oList);
        }

        [HttpGet]
        [Route("api/players/Read")]
        public IHttpActionResult Read(int PlayerID)
        {
            Player oItem = null;

            oItem = oDA.Read(PlayerID);
            if (oItem == null)
            {
                return NotFound();
            }
            return Ok(oItem);
        }

        [HttpPut]
        [Route("api/players/Save")]
        public IHttpActionResult Save(Player player)
        {
            oDA.Save(player);
            return Ok();

        }

        [HttpGet]
        [Route("api/players/Remove")]
        public IHttpActionResult Remove(int PlayerID)
        {

            oDA.Remove(PlayerID);
            return Ok();
        }
    }
}
