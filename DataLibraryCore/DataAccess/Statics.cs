using Dapper;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess
{
    internal static class Statics
    {
        public static InvoiceModel MapToSingleInvoice(this SqlMapper.GridReader reader)
        {
            InvoiceModel invoice = reader.Read<InvoiceModel, CustomerModel, InvoiceModel>(
                (i, c) => { i.Customer = c; return i; }, splitOn: "CustId").Single();

            var ItemsDic = reader.Read<InvoiceItemModel, ProductModel, InvoiceItemModel>(
                (it, p) => { it.Product = p; return it; }, splitOn: "pId")
                .GroupBy(s => s.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PaymentsDic = reader
                .Read<InvoicePaymentModel>()
                .GroupBy(t => t.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PhonesDic = reader
                 .Read<PhoneNumberModel>()
                 .GroupBy(s => s.CustomerId)
                 .ToDictionary(g => g.Key, g => g.AsEnumerable());

            if (ItemsDic.TryGetValue(invoice.Id, out IEnumerable<InvoiceItemModel> items))
                invoice.Items = new(items);
            if (PaymentsDic.TryGetValue(invoice.Id, out IEnumerable<InvoicePaymentModel> payments))
                invoice.Payments = new(payments);
            if (PhonesDic.TryGetValue(invoice.Customer.Id, out IEnumerable<PhoneNumberModel> phones))
                invoice.Customer.PhoneNumbers = new(phones);
            return invoice;
        }

        public static InvoiceModel MapToSingleInvoiceAsync(this Task<SqlMapper.GridReader> reader)
        {
            InvoiceModel invoice = reader.Result.Read<InvoiceModel, CustomerModel, InvoiceModel>(
                (i, c) => { i.Customer = c; return i; }, splitOn: "CustId").Single();

            var ItemsDic = reader.Result.Read<InvoiceItemModel, ProductModel, InvoiceItemModel>(
                (it, p) => { it.Product = p; return it; }, splitOn: "pId")
                .GroupBy(s => s.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PaymentsDic = reader.Result
                .Read<InvoicePaymentModel>()
                .GroupBy(t => t.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PhonesDic = reader.Result
                 .Read<PhoneNumberModel>()
                 .GroupBy(s => s.CustomerId)
                 .ToDictionary(g => g.Key, g => g.AsEnumerable());

            if (ItemsDic.TryGetValue(invoice.Id, out IEnumerable<InvoiceItemModel> items))
                invoice.Items = new(items);
            if (PaymentsDic.TryGetValue(invoice.Id, out IEnumerable<InvoicePaymentModel> payments))
                invoice.Payments = new(payments);
            if (PhonesDic.TryGetValue(invoice.Customer.Id, out IEnumerable<PhoneNumberModel> phones))
                invoice.Customer.PhoneNumbers = new(phones);
            return invoice;
        }

        internal static ObservableCollection<InvoiceModel> MapObservableCollectionOfInvoices(this SqlMapper.GridReader reader)
        {
            ObservableCollection<InvoiceModel> invoices = new(reader.Read<InvoiceModel, CustomerModel, InvoiceModel>(
                (i, c) => { i.Customer = c; return i; }, splitOn: "CustId"));
            var ItemsDic = reader.Read<InvoiceItemModel, ProductModel, InvoiceItemModel>(
                (it, p) => { it.Product = p; return it; }, splitOn: "pId")
                .GroupBy(s => s.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PaymentsDic = reader
                .Read<InvoicePaymentModel>()
                .GroupBy(t => t.InvoiceId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            var PhonesDic = reader
                 .Read<PhoneNumberModel>()
                 .GroupBy(s => s.CustomerId)
                 .ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var invoice in invoices)
            {
                if (ItemsDic.TryGetValue(invoice.Id, out IEnumerable<InvoiceItemModel> items))
                    invoice.Items = new(items);
                if (PaymentsDic.TryGetValue(invoice.Id, out IEnumerable<InvoicePaymentModel> payments))
                    invoice.Payments = new(payments);
                if (PhonesDic.TryGetValue(invoice.Customer.Id, out IEnumerable<PhoneNumberModel> phones))
                    invoice.Customer.PhoneNumbers = new(phones);
            }
            return invoices;
        }

        internal static ObservableCollection<CustomerModel> MapObservableCollectionOfCustomers
        (
            this SqlMapper.GridReader reader
        )
        {
            var first = new ObservableCollection<CustomerModel>(reader.Read<CustomerModel>());
            var childMap = reader
                .Read<PhoneNumberModel>()
                .GroupBy(s => s.CustomerId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var item in first)
                if (childMap.TryGetValue(item.Id, out IEnumerable<PhoneNumberModel> children))
                    item.PhoneNumbers = new(children);
            return first;
        }

        internal static ObservableCollection<TFirst> MapObservableCollectionOfCheques<TFirst, TSecond, TKey>
        (
            this SqlMapper.GridReader reader,
            Func<TFirst, TKey> firstKey,
            Func<TSecond, TKey> secondKey,
            Action<TFirst, IEnumerable<TSecond>> addChildren
        )
        {
            var first = new ObservableCollection<TFirst>(reader.Read<TFirst>());
            var childMap = reader
                .Read<TSecond>()
                .GroupBy(s => secondKey(s))
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var item in first)
            {
                if (childMap.TryGetValue(firstKey(item), out IEnumerable<TSecond> children))
                {
                    addChildren(item, children);
                }
            }
            return first;
        }

        internal static ObservableCollection<T> AsObservable<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        internal static async Task<ObservableCollection<T>> AsObservableAsync<T>(this Task<IEnumerable<T>> collection)
        {
            return await Task.FromResult(new ObservableCollection<T>(collection.Result));
        }
    }
}
