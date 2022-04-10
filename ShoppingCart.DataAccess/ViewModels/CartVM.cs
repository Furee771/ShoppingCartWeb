﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.ViewModels
{
     public class CartVM
     {
        public Cart Cart { get; set; } = new Cart();
        public OrderHeader OrderHeader { get; set; } = new OrderHeader();
        public IEnumerable<Cart> ListOfCart { get; set; } = new List<Cart>();
    }
}
