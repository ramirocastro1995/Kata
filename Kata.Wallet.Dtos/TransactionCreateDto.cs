using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Dtos
{
    public class TransactionCreateDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int WalletIncoming { get; set; }
        public int WalletOutgoing { get; set; }
    }
}
