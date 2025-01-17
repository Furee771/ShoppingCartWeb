﻿using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{ 
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
		{
			var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
			orderHeader.DateofPayment = DateTime.Now;
			orderHeader.PaymentIntentId = PaymentIntentId;
			orderHeader.SessionId = SessionId;
		}

		public void Update(OrderHeader orderHeader)
		{
			_context.OrderHeaders.Update(orderHeader);
			//var categoryDB = _context.Categories.FirstOrDefault(20000000);
			//if (categoryDB != null)
			//{
			// categoryDB.Name = category.Name;
			// categoryDB.DisplayOrder = category.DisplayOrder;
			//}
		}

		public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
		{
			var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
			if (order != null)
			{
				order.OrderStatus = orderStatus;
			}
			if (paymentStatus != null)
			{
				order.PaymentStatus = paymentStatus;
			}
		}
	}
}
