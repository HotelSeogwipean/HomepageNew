﻿using Seogwipean.Data.Models;
using Seogwipean.Model.CouponViewModels;
using Seogwipean.Model.KakaoViewModels;
using Seogwipean.Model.ResultModels;
using Seogwipean.Model.SurfViewModels;
using System.Collections.Generic;

namespace Seogwipean.Data.Repositories.Interface
{
    public interface ICouponRepository
    {
        Coupon GetCoupon(long Id);
        CouponViewModel GetCouponModel(long Id);
        LongResult<IList<Coupon>> GetCouponList(CouponViewModel vm);
        LongResult<CouponViewModel> CreateCoupon(CouponViewModel vm);
        LongResult<CouponViewModel> UseCoupon(string phone);
        CouponViewModel GetCouponKakao(KakaoViewModel vm);
        CouponDb GetCouponDB(long Id);
        CouponDb UpdateCouponDB(CouponDBViewModel vm);
    }
}
