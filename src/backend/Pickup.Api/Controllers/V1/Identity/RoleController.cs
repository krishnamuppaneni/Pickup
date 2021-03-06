﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pickup.Core.Models.V1.Request.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Controllers.V1.Identity
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/role")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }


        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IdentityRole>), 200)]
        [Route("get")]
        public IActionResult Get() => Ok(_roleManager.Roles);

        /// <summary>
        /// Insert a role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("insert")]
        public async Task<IActionResult> Post([FromBody]RoleRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            IdentityRole identityRole = new IdentityRole
            {
                Name = model.Name
            };

            IdentityResult result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="Id">Role Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("update/{Id}")]
        public async Task<IActionResult> Put(int Id, [FromBody]RoleRequest model)
        {
            IdentityRole identityRole = await _roleManager.FindByIdAsync(model.Id);

            identityRole.Name = model.Name;

            IdentityResult result = await _roleManager.UpdateAsync(identityRole);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("delete/{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest(new string[] { "Could not complete request!" });

            IdentityRole identityRole = await _roleManager.FindByIdAsync(Id);
            if (identityRole == null)
                return BadRequest(new string[] { "Could not find role!" });

            IdentityResult result = _roleManager.DeleteAsync(identityRole).Result;
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }
    }
}