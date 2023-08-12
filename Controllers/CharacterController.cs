using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        // private static Character knight = new Character();

        // [HttpGet]
        // public IActionResult Get()
        // {
        //     // return BadRequest(knight);
        //     // return NotFound(knight);
        //     return Ok(knight);
        // }

        // [HttpGet]
        // public ActionResult<Character> Get()
        // {
        //     // return BadRequest(knight);
        //     // return NotFound(knight);
        //     return Ok(knight);
        // }

        ///////////////////////////////////////////////

        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character {Id = 1, Name = "Sam"}
        };

        [HttpGet("GetAll")]
        public ActionResult<List<Character>> Get()
        {
            return Ok(characters);
        }

        // [HttpGet]
        // public ActionResult<Character> GetSingle()
        // {
        //     return Ok(characters[0]);
        // }

        [HttpGet("{id}")]
        public ActionResult<Character> GetSingle(int id)
        {
            // return Ok(characters[0]);
            return Ok(characters.FirstOrDefault(c => c.Id == id));
        }

        [HttpPost]
        public ActionResult<List<Character>> AddCharacter(Character newCharacter)
        {
            characters.Add(newCharacter);
            return Ok(characters);
        }

    }
}