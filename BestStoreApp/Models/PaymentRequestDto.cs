﻿namespace BestStoreApp.Models;

public class PaymentRequestDto
{
    public string Token {  get; set; }=string.Empty;
    public long Amount {  get; set; }
    public string DeliveredAddress {  get; set; }   =string.Empty;
}
