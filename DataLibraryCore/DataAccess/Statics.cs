﻿using Dapper;
using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess;

internal static class Statics
{
    public static InvoiceModel MapToSingleInvoice(this SqlMapper.GridReader reader)
    {
        InvoiceModel invoice = reader.Read<InvoiceModel, CustomerModel, InvoiceModel>(
            (i, c) => { i.Customer = c; return i; }, splitOn: "CustId").SingleOrDefault();
        if (invoice is null) return null;

        var ItemsDic = reader.Read<InvoiceItemModel, ProductModel, ProductUnitModel, InvoiceItemModel>(
            (it, p, u) => { it.Product = p; it.Unit = u; return it; }, splitOn: "pId, puId")
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

    internal async static Task<List<CustomerModel>> MapObservableCollectionOfCustomersAsync
    (
        this SqlMapper.GridReader reader
    )
    {
        var first = (await reader.ReadAsync<CustomerModel>()).AsList();
        var task = await reader.ReadAsync<PhoneNumberModel>();
        var childMap = task
            .GroupBy(s => s.CustomerId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        foreach (var item in first)
            if (childMap.TryGetValue(item.Id, out IEnumerable<PhoneNumberModel> children))
                item.PhoneNumbers = new(children);
        return first;
    }

    internal async static Task<List<TFirst>> MapObservableCollectionOfChequesAsync<TFirst, TSecond, TKey>
    (
        this SqlMapper.GridReader reader,
        Func<TFirst, TKey> firstKey,
        Func<TSecond, TKey> secondKey,
        Action<TFirst, IEnumerable<TSecond>> addChildren
    )
    {
        var first = new List<TFirst>(await reader.ReadAsync<TFirst>());
        var task = await reader.ReadAsync<TSecond>();
        var childMap = task.GroupBy(s => secondKey(s))
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

    public static TransactionModel MapToSingleTransaction(this SqlMapper.GridReader reader)
    {
        TransactionModel transaction = reader.Read<TransactionModel>().SingleOrDefault();
        if (transaction is null) return null;

        var ItemsDic = reader.Read<TransactionItemModel>()
            .GroupBy(s => s.TransactionId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        if (ItemsDic.TryGetValue(transaction.Id, out IEnumerable<TransactionItemModel> items))
            transaction.Items = new(items);
        return transaction;
    }
}