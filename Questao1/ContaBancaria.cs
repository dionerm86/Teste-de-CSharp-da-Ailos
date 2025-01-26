using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria {

        private const double TaxaDeSaque = 3.50;
        private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

        public ContaBancaria(int numero, string titular, double? depositoInicial = null)
        {
            Numero = numero;
            Titular = titular;
            Saldo = 0.0;
            
            if (depositoInicial.HasValue)
                Deposito(depositoInicial.Value);
            
        }

        public int Numero { get; private set; }
        public string Titular { get; set; }
        public double Saldo { get; private set; }

        public void Deposito(double quantia)
        { 
            Saldo += quantia <= 0 ? throw new ArgumentException("O valor do depósito deve ser maior que 0") : quantia;
        }

        public void Saque(double quantia)
        {
            Saldo -= quantia <= 0 ? throw new ArgumentException("O valor do depósito deve ser maior que 0"): quantia + TaxaDeSaque;
        }

        public override string ToString()
        {
            return $"Conta: {Numero}, Titular: {Titular}, Saldo: {Saldo.ToString("C", UsCulture)}";
        }

    }
}
