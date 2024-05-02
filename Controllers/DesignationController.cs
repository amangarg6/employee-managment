using AutoMapper;
using FluentValidation;
using Login_Register.DTO_s;
//using Login_Register.Migrations;
using Login_Register.Models;
using Login_Register.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login_Register.Controllers
{
    [Route("api/designation")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly IMapper _mapper;
        public DesignationController(IDesignationRepository designationRepository, IMapper mapper)
        {
            _designationRepository = designationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var designations = await _designationRepository.GetDesignations();
            var designationdto = designations.Select(_mapper.Map<Designation, Designationdto>).ToList();
            return Ok(designationdto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Designationdto designationdto)
        {
            if (designationdto == null) return BadRequest(ModelState);
            ////fluentvalidation
            var validator = new DesignationValidator();
            var validationresult = validator.Validate(designationdto);
            if (!validationresult.IsValid)
            {
                return BadRequest(validationresult.Errors);
            }
            ////**

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var designation = _mapper.Map<Designationdto, Designation>(designationdto);
            if (designation == null) return NotFound();
            var data = _designationRepository.CreateDesignation(designation);
            return Ok(new { Status = "Success", data = designation, Message = "Designation created successfully!" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Designationdto designationdto)
        {
            if (designationdto == null) return BadRequest(ModelState);
            ////fluentvalidation
            var validator = new DesignationValidator();
            var validationresult = validator.Validate(designationdto);
            if (!validationresult.IsValid)
            {
                return BadRequest(validationresult.Errors);
            }
            ////**

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var designation = _mapper.Map<Designationdto, Designation>(designationdto);
            if (!await _designationRepository.UpdateDesignation(designation))
            {
                ModelState.AddModelError("", $"Something went wrong Update designation:{designation.Name} ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = "Success", data = designationdto, Message = "Designation Update successfully!" });
        }

        [HttpDelete("{designationid:int}")]
        public async Task<IActionResult> delete(int designationid)
        {
            var designation = await _designationRepository.GetDesignation(designationid);
            if (designation == null) return NotFound();
            if (!await _designationRepository.DeleteDesignation(designation))
            {
                ModelState.AddModelError("", $"something went wrong while Delete designation:{designation.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Status = "Success", Message = "Designation Delete successfully!" });
        }
        [HttpGet]
        [Route("designation")]
        public async Task<IActionResult> GetDesignationbyEmployeeId(int id)
        {
            var designation = await _designationRepository.GetDesignationByEmployeeId(id);
            if (designation == null || !designation.Any())
            {
                return NotFound("No Designation found for the specified Employee ID");
            }
            return Ok(designation);
        }

    }
}
