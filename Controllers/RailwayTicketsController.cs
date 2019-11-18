using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayApi.Models;

// TODO: check date
// TODO: remove token after it was used
// TODO: how to register my service ?

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
                if (token.token.Equals(tokenContents)) {
                    return token;
                }
            }
            return null;
        }

        [HttpGet("token")]
        public ActionResult<IEnumerable<Token>> GetTokens()
        {
            return tokens;
        } 

        [HttpPost("token")]
        public ActionResult<Token> AddToken(Token token) {
            tokens.Add(token);
            return token;
        }

        // PUT: api/RailwayTickets/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}/{token}")]
        public async Task<IActionResult> PutTicket(long id, Ticket ticket, string token)
        {
            Token referenceToken = findTokenByContents(token);
            if (referenceToken == null ||
                !referenceToken.methodName.Equals("PutTicket")) {
                // TODO: also check date
                return BadRequest(); // TODO: probably change to something else
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
            Token referenceToken = findTokenByContents(token);
            if (referenceToken == null ||
                !referenceToken.methodName.Equals("PostTicket")) {
                // TODO: also check date
                return BadRequest();
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
