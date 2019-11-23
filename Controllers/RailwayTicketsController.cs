using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayApi.Models;

/*
{
	"from" : "Minsk",
	"to" : "Moscow",
	"when" : "2019-11-29T14:51:00Z",
	"price" : 60.87
}

{
	"token" : "4rty",
	"date_from" : "12-12-2000",
	"date_to" : "12-12-2019"
}
*/

namespace RailwayApi.Controllers
{
    [Route("api/RailwayTickets")]
    [ApiController]
    public class RailwayTicketsController : ControllerBase
    {
        private readonly RailwayContext _context;

        public RailwayTicketsController(RailwayContext context)
        {
            _context = context;
        }

        // GET: api/RailwayTickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Gettickets()
        {
            return await _context.tickets.ToListAsync();
        }

        // GET: api/RailwayTickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(long id)
        {
            var ticket = await _context.tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        private static List< Token > tokens = new List< Token >();

        private Token findTokenByContents(String tokenContents) {
            foreach (Token token in tokens) {
                if (token.tokenInfo.token.Equals(tokenContents)) {
                    return token;
                }
            }
            return null;
        }

        private void removeToken(string tokenContents) {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].tokenInfo.token.Equals(tokenContents)) {
                    tokens.RemoveAt(i);
                    break;
                }
            }
        }

        [HttpGet("token")]
        public ActionResult<IEnumerable<Token>> GetTokens()
        {
            return tokens;
        } 

        [HttpPost("token/{methodName}")]
        public ActionResult<Token> AddToken(TokenInfo tokenInfo, string methodName) {
            Token token = new Token();
            token.tokenInfo = tokenInfo;
            token.methodName = methodName;
            tokens.Add(token);
            return token;
        }

        private Boolean checkToken(string token, string methodName) {
            Token referenceToken = findTokenByContents(token);
            if (referenceToken == null) {
                return false;
            }

            DateTime dateFrom = DateTime.ParseExact(referenceToken.tokenInfo.date_from, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateTo = DateTime.ParseExact(referenceToken.tokenInfo.date_to, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            if (!referenceToken.methodName.Equals(methodName, StringComparison.InvariantCultureIgnoreCase) ||
                dateFrom > DateTime.Now || dateTo < DateTime.Now) {
                return false;
            }

            removeToken(token);

            return true;
        }

        // PUT: api/RailwayTickets/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}/{token}")]
        public async Task<IActionResult> PutTicket(long id, Ticket ticket, string token)
        {
            if (!checkToken(token, "PutTicket")) {
                return StatusCode(403);
            }

            if (id != ticket.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RailwayTickets
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("{token}")]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket, string token)
        {
            if (!checkToken(token, "PostTicket")) {
                return StatusCode(403);
            }

            _context.tickets.Add(ticket);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        // DELETE: api/RailwayTickets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ticket>> DeleteTicket(long id)
        {
            var ticket = await _context.tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        private bool TicketExists(long id)
        {
            return _context.tickets.Any(e => e.Id == id);
        }
    }
}
