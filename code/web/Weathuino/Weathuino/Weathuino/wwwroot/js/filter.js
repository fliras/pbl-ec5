function applyFilter() {
    // logica do filtro
    alert('Filtrado');
}
function deletaMedidor(id) {
    const deveDeletar = confirm('Tem certeza que deseja excluir este medidor?');
    if (deveDeletar)
        document.location.href = `/Medidores/Delete?id=${id}`;
}