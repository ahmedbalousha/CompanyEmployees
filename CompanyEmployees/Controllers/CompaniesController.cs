using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger,
        IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //[HttpGet]
        //public IActionResult GetCompanies()
        //{
        //    try
        //    {
        //        var companies = _repository.Company.GetAllCompanies(trackChanges: false);
        //        return Ok(companies);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong in the {nameof(GetCompanies)}action { ex}");

        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges:
           false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            
                //throw new Exception("Exception");
            

           return Ok(companiesDto);

        }

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyForCreationDto
        company)
        {
            //if (company == null)
            //{
            //    _logger.LogError("CompanyForCreationDto object sent from client is null.");
            //    return BadRequest("CompanyForCreationDto object is null");
            //}
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogError("Invalid model state for the CompanyForCreationDto object");
            //    return UnprocessableEntity(ModelState);
            //}
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
           await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id },companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =
        typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        //[HttpGet("collection/({ids})", Name = "CompanyCollection")]
        //public IActionResult GetCompanyCollection(IEnumerable<Guid> ids)
        //{
        //    if (ids == null)
        //    {
        //        _logger.LogError("Parameter ids is null");
        //        return BadRequest("Parameter ids is null");
        //    }
        //    var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);
        //    if (ids.Count() != companyEntities.Count())
        //    {
        //        _logger.LogError("Some ids are not valid in a collection");
        //        return NotFound();
        //    }
        //    var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        //    return Ok(companiesToReturn);
        //}

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
           await _repository.SaveAsync();
            var companyCollectionToReturn =
           _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids },companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company;

            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto
        company)
        {
            //if (company == null)
            //{
            //    _logger.LogError("CompanyForUpdateDto object sent from client is null.");
            //    return BadRequest("CompanyForUpdateDto object is null");
            //}
            var companyEntity = HttpContext.Items["company"] as Company;
            
            _mapper.Map(company, companyEntity);
           await _repository.SaveAsync();
            return NoContent();
        }
    }
}
