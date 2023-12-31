﻿using Seogwipean.Data.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Seogwipean.Data.Models;
using System.Linq;
using System;
using Seogwipean.Util;
using Seogwipean.Model.ResultModels;
using System.Collections.Generic;
using Seogwipean.Model.SurfViewModels;
using Microsoft.EntityFrameworkCore;

namespace Seogwipean.Data.Repositories
{
    public class SurfRepository : ISurfRepository
    {
        private readonly ILogger _logger;
        private readonly HotelSeogwipeanDbContextFactory _dbContextFactory;

        public SurfRepository(ILoggerFactory loggerFactory,
            HotelSeogwipeanDbContextFactory dbContextFactory)
        {
            this._logger = loggerFactory.CreateLogger<SurfRepository>();
            this._dbContextFactory = dbContextFactory;
        }

        public Surf GetSurf(long Id)
        {
            try
            {
                using (var db = _dbContextFactory.Create())
                {
                    var result = db.Surf.FirstOrDefault(b => b.Id == Id);
                    if (result == null)
                    {
                        return null;
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                if (e is SeogwipeanException)
                {
                    return null;
                }
                return null;
            }
        }

        public LongResult<IList<Surf>> GetSurfList(SurfViewModel vm)
        {
            try
            {
                using (var db = _dbContextFactory.Create())
                {
                    var isSearch = true;

                    var _list = db.Surf.AsNoTracking();
                    var userName = vm.UserName;
                    var startDate = vm.StartDate;
                    var startTime = vm.StartTime;
                    if (isSearch)
                    {
                        if (!string.IsNullOrEmpty(userName))
                        {
                            _list = _list.Where(b => b.UserName == userName);
                        }
                        if (startDate.Year > 1)
                        {
                            _list = _list.Where(b => b.StartDate >= startDate);
                        }
                    }
                    return new LongResult<IList<Surf>>
                    {
                        Result = Common.Success,
                        Data = _list.OrderBy(b => b.StartTime).OrderBy(b => b.StartDate).ToList()
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                if (e is SeogwipeanException)
                {
                    return new LongResult<IList<Surf>>
                    {
                        Result = Common.Fail,
                        Reason = e.Message
                    };
                }
                return new LongResult<IList<Surf>>
                {
                    Result = Common.Exception,
                    Reason = null
                };
            }
        }

        public LongResult<SurfViewModel> AddSurf(SurfViewModel vm)
        {
            try
            {
                using (var db = _dbContextFactory.Create())
                {
                    long Id = vm.Id;
                    string userName = vm.UserName;
                    string email = vm.Email;
                    string phone = vm.Phone;
                    int headCount = vm.HeadCount;
                    string request = vm.Request;
                    int? ageRange = vm.AgeRange;
                    DateTime startDate = vm.StartDate;
                    int startTime = vm.StartTime;

                    if (vm == null)
                    {
                        return new LongResult<SurfViewModel>
                        {
                            Result = Common.Fail,
                            Reason = "데이터가 존재하지 않습니다."
                        };
                    }

                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        throw new SeogwipeanException("예약자명이 존재하지 않습니다.");
                    }
                    if (headCount < 1)
                    {
                        throw new SeogwipeanException("인원수가 존재하지 않습니다.");
                    }

                    var newBooking = new Surf
                    {
                        UserName = userName,
                        HeadCount = headCount,
                        Email = email,
                        Request = request,
                        Phone = phone,
                        StartDate = startDate,
                        StartTime = startTime,
                        Status = CodesName.Booking_Booked,
                        AgeRange = ageRange,
                        CreateDate = DateTime.Now
                    };
                    db.Surf.Add(newBooking);
                    db.SaveChanges();

                    _logger.LogError(DateTime.Now + " || 서핑 예약 추가, 예약자명: " + userName + ", 체크인: " + startDate.ToString());
                    return new LongResult<SurfViewModel>
                    {
                        Result = Common.Success,
                        Data = new SurfViewModel
                        {
                            UserName = userName,
                            HeadCount = headCount,
                            Email = email,
                            Request = request,
                            Phone = phone,
                            StartDate = startDate,
                            StartTime = startTime,
                            Status = CodesName.Booking_Booked,
                            AgeRange = ageRange
                        }
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                if (e is SeogwipeanException)
                {
                    return new LongResult<SurfViewModel>
                    {
                        Result = Common.Fail,
                        Reason = e.Message
                    };
                }
                return new LongResult<SurfViewModel>
                {
                    Result = Common.Exception,
                    Reason = null
                };
            }
        }


        public LongResult<SurfViewModel> UpdateSurf(SurfViewModel vm)
        {
            try
            {
                using (var db = _dbContextFactory.Create())
                {
                    var surfId = vm.Id;
                    var userName = vm.UserName;
                    var email = vm.Email;
                    var phone = vm.Phone;
                    var startDate = vm.StartDate;
                    var startTime = vm.StartTime;
                    var request = vm.Request;
                    var headCount = vm.HeadCount;
                    var ageRange = vm.AgeRange;

                    var _booking = db.Surf.FirstOrDefault(b => b.Id == surfId);
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        _booking.UserName = userName;
                    }
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        _booking.Email = email;
                    }
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        _booking.Phone = phone;
                    }
                    if (startDate.Year > 1)
                    {
                        _booking.StartDate = startDate;
                    }
                    if (startTime > 0)
                    {
                        _booking.StartTime = startTime;
                    }
                    if (!string.IsNullOrWhiteSpace(request))
                    {
                        _booking.Request = request;
                    }
                    if (vm.Status > 0)
                    {
                        _booking.Status = vm.Status;
                    }
                    if (headCount > 0)
                    {
                        _booking.HeadCount = headCount;
                    }
                    db.Update(_booking);
                    var _result = db.SaveChanges();
                    return new LongResult<SurfViewModel>
                    {
                        Result = Common.Success,
                        Data = new SurfViewModel
                        {
                            UserName = userName,
                            HeadCount = headCount,
                            Email = email,
                            Request = request,
                            Phone = phone,
                            StartDate = startDate,
                            StartTime = startTime,
                            Status = CodesName.Booking_Booked,
                            AgeRange = ageRange
                        }
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                if (e is SeogwipeanException)
                {
                    return new LongResult<SurfViewModel>
                    {
                        Result = Common.Fail,
                        Reason = e.Message
                    };
                }
                return new LongResult<SurfViewModel>
                {
                    Result = Common.Exception,
                    Reason = null
                };
            }
        }


    }

}