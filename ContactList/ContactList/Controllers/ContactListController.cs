using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactList.Controllers
{
    public class Person
    {
        [Required]
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
    }
    [ApiController]
    [Route("api/contacts")]
    public class ContactListController : ControllerBase
    {
        private static readonly List<Person> contacts = new List<Person>
        {
             new Person { Firstname = "Michael", Lastname = "Hitzker", Email="michael@michaelhitzker.com", Id = 1 }
        };

        [HttpGet]
        public IActionResult GetAllItems()
        {
            return Ok(contacts);
        }

        [HttpGet]
        [Route("{id}", Name = "GetSpecificItem")]
        public IActionResult GetSpecificItem(int id)
        {
            Person person = contacts.Find(person => person.Id == id);
            if (person != null)
            {
                return Ok(person);

            }
            return BadRequest("Invalid Index");
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] Person newContact)
        {
            int newId = newContact.Id;
            if (newId == null)
            {
                return BadRequest("Please enter an Id");
            }
            Person existingPerson = contacts.Find(person => person.Id == newId);
            if(existingPerson != null)
            {
                return BadRequest("That Id already exists!");
            }
            contacts.Add(newContact);
            return CreatedAtRoute("GetSpecificItem", new { id = newId }, newContact);
        }

        [HttpDelete]
        [Route("{id}", Name = "deletePerson")]

        public IActionResult RemoveItem(int id)
        {
            int index= contacts.FindIndex(person => person.Id == id);
            if(index < 0)
            {
                return BadRequest("That id does not exist!");
            }
            contacts.RemoveAt(index);
            return NoContent();
        }

        [HttpGet]
        [Route("findByName", Name = "FindByName")]
        public IActionResult FindByName([FromQuery]string nameFilter)
        {
            if (nameFilter == null || nameFilter.Length<=0)
            {
                return BadRequest("Please enter a nameFilter");
            }
            IEnumerable<Person> people = from person in contacts
                         where person.Firstname.ToLower().Contains(nameFilter.ToLower()) || person.Lastname.ToLower().Contains(nameFilter.ToLower())
                         select person;
            if(people.Count() > 0)
            {
                return Ok(people);
            }
            return BadRequest("Name does not exist");
        }
    }
}
