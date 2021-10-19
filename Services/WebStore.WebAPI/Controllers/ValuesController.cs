using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebAPI.Controllers
{

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
        public IActionResult Add([FromBody] string value)
        {
            var id = _Values.Count == 0 ? 1 : _Values.Keys.Max() + 1;
            _Values[id] = value;

            var r = CreatedAtAction(nameof(GetById), new { id = id }, _Values[id]);
            return r;
        }

        [HttpPut("{id}")]
        public IActionResult Replace(int id, [FromBody] string value)
        {
            if (!_Values.ContainsKey(id))
                return NotFound();

            _Values[id] = value;

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_Values.ContainsKey(id))
                return NotFound();

            _Values.Remove(id);
            return Ok();
        }
    }
}
