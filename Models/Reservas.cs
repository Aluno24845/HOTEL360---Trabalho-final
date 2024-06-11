namespace HOTEL360___Trabalho_final.Models
{
    public class Reservas {
        public int Id { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataReserva {  get; set; }
        public DateTime DataCheckIN { get; set; }
        public DateTime DataCheckOUT { get; set; }
    }
}
