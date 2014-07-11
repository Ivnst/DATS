﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DATS
{
    public class PriceView

    {
        public PriceView()
        {

        }

        public PriceView(int stadiumId, int matchId, string name, decimal priceValue)
        {
            this.StadiumId = stadiumId;
            this.MatchId = matchId;
            this.Name = name;
            this.PriceValue = priceValue;
        }


        public int? StadiumId { get; set; }
        public int? MatchId { get; set; }

        [DisplayName("Название сектора")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Пожалуйста введите название сектора.")]
        [StringLength(255)]
        public string Name { get; set; }

        [DisplayName("Цена")]
        [Range(0, 1000000000, ErrorMessage = @"Поле ""Продолжительность в минутах"" должно содержать целое число большее нуля.")]
        [Required(ErrorMessage = "Пожалуйста введите продолжительность в минутах.")]
        public decimal? PriceValue { get; set; }

    }
}