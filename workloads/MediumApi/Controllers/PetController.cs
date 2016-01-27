﻿using System;
using MediumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediumApi.Controllers
{
    [Route("/pet")]
    public class PetController : BaseController
    {
        public PetController(PetRepository repository)
        {
            Repository = repository;
        }

        public PetRepository Repository { get; }

        [HttpGet("{id}", Name = "FindPetById")]
        public IActionResult FindById(int id)
        {
            var pet = Repository.FindPet(id);
            if (pet == null)
            {
                return new HttpNotFoundResult();
            }

            return new ObjectResult(pet);
        }

        [HttpGet("findByStatus")]
        public IActionResult FindByStatus(string status)
        {
            throw new NotImplementedException();
        }

        [HttpGet("findByTags")]
        public IActionResult FindByTags(string[] tags)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult AddPet([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            Repository.AddPet(pet);

            return new CreatedAtRouteResult("FindPetById", new { id = pet.Id }, pet);
        }

        [HttpPut]
        public IActionResult EditPet(Pet pet)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}")]
        public IActionResult EditPetForm(int id, Pet pet)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/uploadImage")]
        public IActionResult UploadImage(int id, IFormFile file)
        {
            throw new NotImplementedException();
        }


        [HttpDelete("{id}")]
        public IActionResult DeletePet(int id)
        {
            throw new NotImplementedException();
        }
    }
}
