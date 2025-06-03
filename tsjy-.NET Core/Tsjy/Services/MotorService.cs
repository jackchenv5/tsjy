using System;
using Faoem.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Tsjy.DbContexts;
using Tsjy.Dtos;
using Tsjy.Models;

namespace Tsjy.Services;

public class MotorService(
    TsjyDbContext tsjyDbContext
)
{

    public async Task<TsjyMotor> AddMotorAsync(TsjyMotor input)
    {
        var exist = await tsjyDbContext.Motors.AnyAsync(motor =>
            motor.FacilityId == input.FacilityId && motor.LowerName == input.Name.ToLower());
        if (exist)
        {
            throw new AppException("Motor already exists", 400);
        }

        await tsjyDbContext.Motors.AddAsync(input);
        await tsjyDbContext.SaveChangesAsync();
        return input;
    }

    public async Task<List<TsjyMotor>> GetMotorsAsync(long facilityId)
    {
        return await tsjyDbContext.Motors.Where(motor => motor.FacilityId == facilityId)
            .ToListAsync();
    }

    public async Task UpdateMotorAsync(TsjyMotor updateMotorDto)
    {
        var motor = await tsjyDbContext.Motors.FindAsync(updateMotorDto.Id);
        if (motor is null)
        {
            throw new AppException("Motor not found", 404);
        }

        motor.Name = updateMotorDto.Name;
        await tsjyDbContext.SaveChangesAsync();
    }

    public async Task DeleteMotorAsync(long motorId)
    {
        var motor = await tsjyDbContext.Motors.FindAsync(motorId);
        if (motor is null)
        {
            throw new AppException("Motor not found", 404);
        }

        tsjyDbContext.Remove(motor);
        await tsjyDbContext.SaveChangesAsync();
    }

}