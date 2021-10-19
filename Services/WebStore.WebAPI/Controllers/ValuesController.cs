using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebAPI.Controllers
{
    //class TestEntity
    //{
    //    public int Id { get; set; }
    //    public string Value { get; set; }
    //}


    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {

        }

        private static readonly Dictionary<int, string> _Values = Enumerable.Range(1, 10)
            .Select(i => (Id: i, Value: $"Value-{i}"))
            .ToDictionary(v => v.Id, v => v.Value);


        [HttpGet("Count")]
        public IActionResult Count() => Ok(_Values.Count);

        [HttpGet]
        public IActionResult Get() => Ok(_Values.Values);

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (!_Values.ContainsKey(id))
                return NotFound();

            return Ok(_Values[id]);
        }

        [HttpPost]
        public IActionResult Add([FromBody] string Value)
        {
            var id = _Values.Count == 0 ? 1 : _Values.Keys.Max() + 1;
            _Values[id] = Value;

            var r = CreatedAtAction(nameof(GetById), new { id = id }, _Values[id]);
            return r;
        }

        [HttpPut("{Id}")]
        public IActionResult Replace(int Id, [FromBody] string value)
        {
            if (!_Values.ContainsKey(Id))
                return NotFound();

            _Values[Id] = value;

            return Ok();
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            if (!_Values.ContainsKey(Id))
                return NotFound();

            _Values.Remove(Id);
            return Ok();
        }
    }
}
