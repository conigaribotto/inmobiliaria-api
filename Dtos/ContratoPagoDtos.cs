namespace InmobiliariaAPI.Dtos;

public record ContratoListItemDto(
    int IdContrato,
    DateTime FechaInicio,
    DateTime FechaFin,
    double ValorMensual,
    bool Vigente
);

public record PagoListItemDto(
    int IdPago,
    string Concepto,
    double Importe,
    DateTime Fecha,
    bool Anulado
);

