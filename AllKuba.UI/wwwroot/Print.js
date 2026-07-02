window.imprimirCotizacion = function () {
    document.body.setAttribute('data-print', 'cotizacion');
    window.print();
    document.body.removeAttribute('data-print');
};
 
window.imprimirOrdenProduccion = function () {
    document.body.setAttribute('data-print', 'produccion');
    window.print();
    document.body.removeAttribute('data-print');
};